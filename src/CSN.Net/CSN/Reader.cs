using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Abstraction.Csn
{
	public class Reader
		: IReader
	{
		public static readonly Reader Singleton = new Reader();

		private Reader()
		{
			// private constructor for singleton
		}

		public void Read(StreamReader sr, IRead callback)
		{
			ReadArgs args = new ReadArgs(sr, callback);
			while (args.State.ReadState != ReadStateEnum.End)
			{
				args.State.Read(args);
			}
		}
	}

	enum ReadStateEnum
	{
		NewRecord,
		VersionRecord,
		TypeDefRecord,
		InstanceRecord,
		ArrayRecord,
		End,
		WhatNext,
		Field
	}

	class ReadArgs
	{
		// parser vars
		public StreamReader Stream = null;
		public IRead Read = null;
		public ReadStateBase State = ReadStateVersionRecord.Singleton;

		// data
		public RecordCode CurrentRC = null;
		public ValueRecord ValueRec = null;
		public Dictionary<long, Record> dcRecords = new Dictionary<long, Record>();

		public ReadArgs(StreamReader pStream, IRead pRead)
		{
			this.Stream = pStream;
			this.Read = pRead;
		}

		public void SetupRecord(Record rec)
		{
			dcRecords[rec.Code.SequenceNo] = rec;
			this.ValueRec = rec as ValueRecord;
		}
	}

	abstract class ReadStateBase
	{
		public ReadStateBase(ReadStateEnum pState)
		{
			this.ReadState = pState;
		}

		public ReadStateEnum ReadState { get; private set; }

		abstract public void Read(ReadArgs args);

		//protected IEnumerable<int> ReadTill(ReadArgs args, int tillChar, bool includeTillChar = false)
		//{
		//	LinkedList<int> charInts = new LinkedList<int>();

		//	int readChar = -1;
		//	while ((readChar = args.Stream.Read()) != -1)
		//	{
		//		if (readChar != tillChar || includeTillChar)
		//		{
		//			charInts.AddLast(readChar);
		//		}
		//		if (readChar == tillChar)
		//		{
		//			break;
		//		}
		//	}

		//	return charInts;
		//}

		protected IEnumerable<int> ReadTill(ReadArgs args, int[] tillChars)
		{
			LinkedList<int> charInts = new LinkedList<int>();

			int readChar = -1;
			while ((readChar = args.Stream.Read()) != -1)
			{
				if (tillChars.Contains(readChar))
				{
					break;
				}
				charInts.AddLast(readChar);
			}
			if (readChar == ReaderHelper.iFieldSep)
			{
				args.State = ReadStateField.Singleton;
			}
			else if (readChar == ReaderHelper.iRecordSep)
			{
				args.State = ReadStateNewRecord.Singleton;
			}
			else if (readChar == -1)
			{
				args.State = ReadStateEnd.Singleton;
			}

			return charInts;
		}


		protected string ReadStringField(ReadArgs args, bool expectOpenEncl = true)
		{
			int readChar = 0;
			// opening encloser
			if (expectOpenEncl)
			{
				readChar = args.Stream.Read();
				if (readChar != ReaderHelper.iStringEncl)
				{
					throw Error.UnexpectedChars(Constants.StringFieldEncloser, Convert.ToChar(readChar));
				}
			}

			StringBuilder sb = new StringBuilder();
			bool escapeState = false;
			while ((readChar = args.Stream.Read()) != -1)
			{
				if (escapeState)
				{
					escapeState = false;
					if (readChar == ReaderHelper.iStringEsc)
					{
						sb.Append(Constants.StringEscapeChar);
					}
					else if (readChar == ReaderHelper.iStringEncl)
					{
						sb.Append(Constants.StringFieldEncloser);
					}
					else if (readChar == -1)
					{
						throw new Error(ErrorCode.UnexpectedEOF);
					}
					else
					{
						throw new Error(ErrorCode.NotEscapeChar).AddData(ErrorDataKeys.Actual, Convert.ToChar(readChar));
					}
				}
				else
				{
					if (readChar == ReaderHelper.iStringEncl)
					{
						break;
					}
					else if (readChar == ReaderHelper.iStringEsc)
					{
						escapeState = true;
						continue;
					}
					else if (readChar == -1)
					{
						throw new Error(ErrorCode.UnexpectedEOF);
					}
					else
					{
						sb.Append(Convert.ToChar(readChar));
					}
				}
			}

			return sb.ToString();
		}

		protected void ReadRecordCode(ReadArgs args)
		{
			// read & Assign RecordCode
			int readChar = args.Stream.Read();

			if (readChar == -1)
			{
				throw new Error(ErrorCode.UnexpectedEOF);
			}

			RecordType recType = ReaderHelper.ResolveRecordType(readChar);
			switch (recType)
			{
				case RecordType.Version:
				case RecordType.Array:
				case RecordType.Instance:
				case RecordType.TypeDef:
					args.CurrentRC = new RecordCode(ReaderHelper.ResolveRecordType(readChar), ReadSeqNo(args));
					break;
				default:
					throw new Error(ErrorCode.UnknownRecordType);
			}
		}

		protected Record ReadRef(ReadArgs args, bool checkFirstChar = true)
		{
			int readChar = 0;
			if (checkFirstChar)
			{
				readChar = args.Stream.Read();
				if (readChar == -1)
				{
					throw new Error(ErrorCode.UnexpectedEOF);
				}
				if (readChar != ReaderHelper.iRefPrefix)
				{
					throw Error.UnexpectedChars(Constants.ReferencePrefix, Convert.ToChar(readChar));
				}
			}

			return args.dcRecords[ReadSeqNo(args)];
		}

		protected long ReadSeqNo(ReadArgs args)
		{
			int readChar = 0;
			long seqNo = 0;
			long mapValue = 0;
			while ((readChar = args.Stream.Read()) != -1)
			{
				if (ReaderHelper.DigitMap.TryGetValue(readChar, out mapValue))
				{
					seqNo = (seqNo * 10) + mapValue;
				}
				else if (readChar == ReaderHelper.iFieldSep)
				{
					args.State = ReadStateField.Singleton;
					break;
				}
				else if (readChar == ReaderHelper.iRecordSep)
				{
					args.State = ReadStateNewRecord.Singleton;
					break;
				}
				else
				{
					throw Error.UnexpectedChars('0', Convert.ToChar(mapValue));
				}
			}
			if (readChar == -1)
			{
				args.State = ReadStateEnd.Singleton;
			}

			return seqNo;
		}

		protected ReadStateBase ReadSkip(StreamReader stream, int skipInitial)
		{
			stream.Read(new char[skipInitial], 0, skipInitial);
			return this;
		}

		public int ReadDateTimeDigits(StreamReader stream, int len)
		{
			int value = 0;
			int readChar = 0;
			int mapValue = 0;
			for (int ctr = 0; ctr < len; ctr++)
			{
				readChar = stream.Read();
				if (ReaderHelper.DigitMapInt.TryGetValue(readChar, out mapValue))
				{
					value = (value * 10) + mapValue;
				}
				else
				{
					throw Error.UnexpectedChars('0', Convert.ToChar(readChar));
				}
			}
			return value;
		}
	}

	class ReadStateNewRecord : ReadStateBase
	{
		public static readonly ReadStateNewRecord Singleton = new ReadStateNewRecord();

		private ReadStateNewRecord()
			: base(ReadStateEnum.NewRecord)
		{ }

		public override void Read(ReadArgs args)
		{
			ReadRecordCode(args);
			// set next state
			switch (args.CurrentRC.RecType)
			{
				case RecordType.Instance:
					args.State = ReadStateInstanceRecord.Singleton;
					break;

				case RecordType.Array:
					args.State = ReadStateArrayRecord.Singleton;
					break;

				case RecordType.TypeDef:
					args.State = ReadStateTypeDefRecord.Singleton;
					break;

				default:
					throw new Error(ErrorCode.UnexpectedRecordType);
			}
		}
	}

	class ReadStateInstanceRecord : ReadStateBase
	{
		public static readonly ReadStateInstanceRecord Singleton = new ReadStateInstanceRecord();

		private ReadStateInstanceRecord() : base(ReadStateEnum.InstanceRecord)
		{ }

		public override void Read(ReadArgs args)
		{
			Record refRec = base.ReadRef(args);
			if (refRec.Code.RecType != RecordType.TypeDef)
			{
				throw new Error(ErrorCode.UnexpectedRecordType);
			}
			InstanceRecord rec = new InstanceRecord(args.CurrentRC, (TypeDefRecord)refRec);
			args.SetupRecord(rec);
			args.Read.Read(rec);

			// base.ReadRef must have already set the next state
		}
	}

	class ReadStateField : ReadStateBase
	{
		public static readonly ReadStateField Singleton = new ReadStateField();
		private ReadStateField() : base(ReadStateEnum.Field)
		{ }

		public override void Read(ReadArgs args)
		{
			IReadValue rv = args.Read.GetReadValue();
			ValueRecord vr = args.ValueRec;

			int readChar = args.Stream.Read();
			// if the field is null
			if (readChar == ReaderHelper.iFieldSep)
			{
				vr.Values.Add(null);
				rv.ReadValueNull(vr, vr.Values.Count);
				return;
			}
			else if (readChar == ReaderHelper.iRecordSep)
			{
				vr.Values.Add(null);
				rv.ReadValueNull(vr, vr.Values.Count);
				args.State = ReadStateNewRecord.Singleton;
				return;
			}
			else if (readChar == -1)
			{
				vr.Values.Add(null);
				rv.ReadValueNull(vr, vr.Values.Count);
				args.State = ReadStateEnd.Singleton;
				return;
			}
			else if (readChar == ReaderHelper.iRefPrefix)
			{
				Record rc = base.ReadRef(args, false);
				if (rc.Code.RecType != RecordType.Instance && rc.Code.RecType != RecordType.Array)
				{
					throw Error.UnexpectedRecordType(RecordType.Instance, rc.Code.RecType);
				}
				vr.Values.Add(rc);
				rv.ReadValue(vr, vr.Values.Count, rc);
				return;
			}
			else if (readChar == ReaderHelper.iStringEncl)
			{
				String str = base.ReadStringField(args, false);
				vr.Values.Add(str);
				rv.ReadValue(vr, vr.Values.Count, str);
			}
			else if (readChar == ReaderHelper.iBoolTrue)
			{
				vr.Values.Add(true);
				rv.ReadValue(vr, vr.Values.Count, true);
			}
			else if (readChar == ReaderHelper.iBoolFalse)
			{
				vr.Values.Add(false);
				rv.ReadValue(vr, vr.Values.Count, false);
			}
			else if (readChar == ReaderHelper.iDateTimePrefix)
			{
				//base.ReadTill(args, new int[] { ReaderHelper.iFieldSep, ReaderHelper.iRecordSep });
				DateTime dt = new DateTime(
					base.ReadDateTimeDigits(args.Stream, 4),
					base.ReadDateTimeDigits(args.Stream, 2),
					base.ReadDateTimeDigits(args.Stream, 2),
					base.ReadSkip(args.Stream, 1).ReadDateTimeDigits(args.Stream, 2),
					base.ReadDateTimeDigits(args.Stream, 2),
					base.ReadDateTimeDigits(args.Stream, 2),
					base.ReadDateTimeDigits(args.Stream, 3)
				);
				vr.Values.Add(dt);
				rv.ReadValue(vr, vr.Values.Count, dt);
			}
			else
			{
				//TODO read number
				base.ReadTill(args, new int[] { ReaderHelper.iFieldSep, ReaderHelper.iRecordSep });
				return;
			}

			// fixed width values (bool, datetime) or values with delimiter (string)
			// still have one extra character to read in order to determine next step
			readChar = args.Stream.Read();
			if (readChar == ReaderHelper.iFieldSep)
			{
				args.State = ReadStateField.Singleton;
			}
			else if (readChar == ReaderHelper.iRecordSep)
			{
				args.State = ReadStateNewRecord.Singleton;
			}
			else if (readChar == -1)
			{
				args.State = ReadStateEnd.Singleton;
			}
			else
			{
				throw Error.UnexpectedChars(Constants.FieldSeparator, Convert.ToChar(readChar));
			}
		}
	}

	class ReadStateArrayRecord : ReadStateBase
	{
		public static readonly ReadStateArrayRecord Singleton = new ReadStateArrayRecord();

		private ReadStateArrayRecord() : base(ReadStateEnum.ArrayRecord) { }

		public override void Read(ReadArgs args)
		{
			// read array type: primitive or typeDef
			int readChar = args.Stream.Read();
			if (readChar == -1)
			{
				throw new Error(ErrorCode.UnexpectedEOF);
			}
			else if (readChar == ReaderHelper.iRefPrefix)
			{
				Record refRec = base.ReadRef(args, false);
				if (refRec.Code.RecType != RecordType.TypeDef)
				{
					throw new Error(ErrorCode.UnexpectedRecordType);
				}
				ArrayRefsRecord rec = new ArrayRefsRecord(args.CurrentRC, (TypeDefRecord)refRec);
				args.SetupRecord(rec);
				args.Read.Read(rec);
			}
			else if (readChar == ReaderHelper.iPrimitivePrefix)
			{
				// read one more char to get primitive type
				readChar = args.Stream.Read();
				ArrayPrimitivesRecod arrRec = new ArrayPrimitivesRecod(args.CurrentRC, ReaderHelper.PrimitiveMap[readChar]);
				args.SetupRecord(arrRec);
				args.Read.Read(arrRec);
				// read a field sep
				readChar = args.Stream.Read();
				if (readChar == -1)
				{
					args.State = ReadStateEnd.Singleton;
				}
				else if (readChar != ReaderHelper.iFieldSep)
				{
					throw Error.UnexpectedChars(Constants.FieldSeparator, Convert.ToChar(readChar));
				}
				else
				{
					args.State = ReadStateField.Singleton;
				}
			}
			else
			{
				throw Error.UnexpectedChars(Constants.RecordTypeChar.Array, Convert.ToChar(readChar));
			}
		}
	}

	class ReadStateTypeDefRecord : ReadStateBase
	{
		public static readonly ReadStateTypeDefRecord Singleton = new ReadStateTypeDefRecord();

		private ReadStateTypeDefRecord() : base(ReadStateEnum.TypeDefRecord) { }

		public override void Read(ReadArgs args)
		{
			// record code has been read,
			TypeDefRecord rec = new TypeDefRecord(args.CurrentRC, base.ReadStringField(args));

			// members
			int readChar = 0;
			List<String> members = new List<string>();
			while ((readChar = args.Stream.Read()) != -1)
			{
				if (readChar == ReaderHelper.iFieldSep)
				{
					members.Add(base.ReadStringField(args));
				}
				else if (readChar == ReaderHelper.iRecordSep)
				{
					rec.Members = members.ToArray();
					args.SetupRecord(rec);
					args.Read.Read(rec);
					args.State = ReadStateNewRecord.Singleton;
					break;
				}
				else
				{
					throw Error.UnexpectedChars(Constants.FieldSeparator, Convert.ToChar(readChar));
				}
			}
			if (readChar == -1)
			{
				args.State = ReadStateEnd.Singleton;
			}
		}
	}

	class ReadStateVersionRecord : ReadStateBase
	{
		public static readonly ReadStateVersionRecord Singleton = new ReadStateVersionRecord();

		private ReadStateVersionRecord() : base(ReadStateEnum.VersionRecord) { }

		public override void Read(ReadArgs args)
		{
			base.ReadRecordCode(args);
			if (args.CurrentRC.RecType != RecordType.Version)
			{
				throw new Error(ErrorCode.UnexpectedRecordType);
			}
			VersionRecord vr = new VersionRecord(args.CurrentRC, base.ReadStringField(args));
			args.dcRecords[vr.Code.SequenceNo] = vr;
			args.Read.Read(vr);

			int readChar = args.Stream.Read();
			if (readChar == -1)
			{
				args.State = ReadStateEnd.Singleton;
			}
			else if (readChar == ReaderHelper.iRecordSep)
			{
				args.State = ReadStateNewRecord.Singleton;
			}
			else
			{
				throw Error.UnexpectedChars(Constants.RecordSeparator, Convert.ToChar(readChar));
			}
		}
	}

	class ReadStateEnd : ReadStateBase
	{
		public static readonly ReadStateEnd Singleton = new ReadStateEnd();

		private ReadStateEnd() : base(ReadStateEnum.End) { }

		public override void Read(ReadArgs args)
		{
			throw new Error(ErrorCode.NothingToRead);
		}
	}

	class ReaderHelper
	{
		public static readonly Dictionary<int, long> DigitMap = null;
		public static readonly Dictionary<int, int> DigitMapInt = null;
		public static readonly Dictionary<int, PrimitiveType> PrimitiveMap = null;

		public static readonly int iVersion = Convert.ToInt32(Constants.RecordTypeChar.Version);
		public static readonly int iTypeDef = Convert.ToInt32(Constants.RecordTypeChar.TypeDef);
		public static readonly int iArray = Convert.ToInt32(Constants.RecordTypeChar.Array);
		public static readonly int iInstance = Convert.ToInt32(Constants.RecordTypeChar.Instance);
		public static readonly int iFieldSep = Convert.ToInt32(Constants.FieldSeparator);
		public static readonly int iRecordSep = Convert.ToInt32(Constants.RecordSeparator);
		public static readonly int iStringEncl = Convert.ToInt32(Constants.StringFieldEncloser);
		public static readonly int iStringEsc = Convert.ToInt32(Constants.StringEscapeChar);
		public static readonly int iRefPrefix = Convert.ToInt32(Constants.ReferencePrefix);
		public static readonly int iBoolTrue = Convert.ToInt32(Constants.BoolTrue);
		public static readonly int iBoolFalse = Convert.ToInt32(Constants.BoolFalse);
		public static readonly int iDateTimePrefix = Convert.ToInt32(Constants.DateTimePrefix);
		public static readonly int iPrimitivePrefix = Convert.ToInt32(Constants.Primitives.Prefix);

		static ReaderHelper()
		{
			PrimitiveMap = new Dictionary<int, PrimitiveType>
			{
				[Convert.ToInt32(Constants.Primitives.Bool)] = PrimitiveType.Bool,
				[Convert.ToInt32(Constants.Primitives.DateTime)] = PrimitiveType.DateTime,
				[Convert.ToInt32(Constants.Primitives.Integer)] = PrimitiveType.Int,
				[Convert.ToInt32(Constants.Primitives.Real)] = PrimitiveType.Real,
				[Convert.ToInt32(Constants.Primitives.String)] = PrimitiveType.String
			};
			DigitMap = new Dictionary<int, long>
			{
				[Convert.ToInt32('0')] = 0L,
				[Convert.ToInt32('1')] = 1L,
				[Convert.ToInt32('2')] = 2L,
				[Convert.ToInt32('3')] = 3L,
				[Convert.ToInt32('4')] = 4L,
				[Convert.ToInt32('5')] = 5L,
				[Convert.ToInt32('6')] = 6L,
				[Convert.ToInt32('7')] = 7L,
				[Convert.ToInt32('8')] = 8L,
				[Convert.ToInt32('9')] = 9L
			};
			DigitMapInt = new Dictionary<int, int>
			{
				[Convert.ToInt32('0')] = 0,
				[Convert.ToInt32('1')] = 1,
				[Convert.ToInt32('2')] = 2,
				[Convert.ToInt32('3')] = 3,
				[Convert.ToInt32('4')] = 4,
				[Convert.ToInt32('5')] = 5,
				[Convert.ToInt32('6')] = 6,
				[Convert.ToInt32('7')] = 7,
				[Convert.ToInt32('8')] = 8,
				[Convert.ToInt32('9')] = 9
			};
		}

		public static RecordType ResolveRecordType(int charInt)
		{
			return (charInt == iInstance) ? RecordType.Instance :
				(charInt == iArray) ? RecordType.Array :
				(charInt == iTypeDef) ? RecordType.TypeDef :
				(charInt == iVersion) ? RecordType.Version :
				RecordType.Unknown;
		}
	}
}
