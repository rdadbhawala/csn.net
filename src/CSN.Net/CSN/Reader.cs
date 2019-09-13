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
			do
			{
				args.State.Read(args);
			} while (args.State != ReadStateEnd.Singleton);
		}
	}

	enum ReadStateEnum
	{
		Unknown,
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
		public ReadStateBase State = ReaderVersionSelector.Singleton;

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
		public ReadStateBase() : this(ReadStateEnum.Unknown)
		{ }

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

		protected String ReadStringStrict(ReadArgs args, bool expectOpenEncl = true)
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
			while (true)
			{
				readChar = args.Stream.Read();
				if (readChar == ReaderHelper.iStringEncl)
				{
					break;
				}
				else if (readChar == ReaderHelper.iStringEsc)
				{
					readChar = args.Stream.Read();
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
					continue;
				}
				else if (readChar >= 0)
				{
					sb.Append(Convert.ToChar(readChar));
				}
				else
				{
					throw new Error(ErrorCode.UnexpectedEOF);
				}
			}

			return sb.ToString();
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

		protected Record ReadRef(ReadArgs args, bool checkFirstChar = true)
		{
			if (checkFirstChar)
			{
				int readChar = args.Stream.Read();
				if (readChar != ReaderHelper.iRefPrefix)
				{
					throw Error.Unexpected(ErrorCode.UnexpectedChars, Constants.ReferencePrefix, readChar);
				}
			}

			return args.dcRecords[ReadSeqNo(args)];
		}

		// for seq no > 0
		protected long ExpectSeqNo(ReadArgs args)
		{
			int expectedSeqNo = args.dcRecords.Count;
			Stack<int> stkSeqNo = new Stack<int>();
			while (expectedSeqNo > 0)
			{
				stkSeqNo.Push(expectedSeqNo % 10);
				expectedSeqNo /= 10;
			}

			int readChar = -1;
			while (stkSeqNo.Count > 0)
			{
				readChar = args.Stream.Read();
				expectedSeqNo = stkSeqNo.Pop();
				if ((expectedSeqNo + ReaderHelper.iDigit0) != readChar)
				{
					throw Error.UnexpectedChars('0', Convert.ToChar(readChar));
				}
			}

			// read the fieldsep
			if ((readChar = args.Stream.Read()) != ReaderHelper.iFieldSep)
			{
				throw Error.UnexpectedChars(Constants.FieldSeparator, Convert.ToChar(readChar));
			}

			return args.dcRecords.Count;
		}

		protected long ReadSeqNo(ReadArgs args)
		{
			int readChar = 0;
			long seqNo = 0;
			long mapValue = 0;
			while (true)
			{
				readChar = args.Stream.Read();
				mapValue = readChar - ReaderHelper.iDigit0;
				if (mapValue >= 0 && mapValue < 10)
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
				else if (readChar == -1)
				{
					args.State = ReadStateEnd.Singleton;
					break;
				}
				else
				{
					throw Error.Unexpected(ErrorCode.UnexpectedChars, readChar, mapValue);
				}
			}

			return seqNo;
		}

		protected ReadStateBase ReadSkipOne(StreamReader stream)
		{
			if (stream.Read() == -1)
			{
				throw new Error(ErrorCode.UnexpectedEOF);
			}
			return this;
		}

		protected ReadStateBase ReadSkip(StreamReader stream, int skipInitial)
		{
			if (stream.Read(new char[skipInitial], 0, skipInitial) != skipInitial)
			{
				throw new Error(ErrorCode.UnexpectedEOF);
			}
			return this;
		}

		public int ReadDateTimeDigits(StreamReader stream, int len)
		{
			int value = 0;
			int mapValue = 0;
			for (int ctr = 0; ctr < len; ctr++)
			{
				int readChar = stream.Read();
				mapValue = readChar - ReaderHelper.iDigit0;
				if (mapValue >= 0 && mapValue < 10)
				{
					value = (value * 10) + mapValue;
				}
				else
				{
					throw Error.Unexpected(ErrorCode.UnexpectedChars, '0', readChar);
				}
			}
			return value;
		}

		protected void ReadExpectNewRecord(ReadArgs args, int readChar)
		{
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

	class ReadStateNewRecord : ReadStateBase
	{
		public static readonly ReadStateNewRecord Singleton = new ReadStateNewRecord();

		private ReadStateNewRecord()
			: base(ReadStateEnum.NewRecord)
		{ }

		public override void Read(ReadArgs args)
		{
			//ReadNonVersionRecordCode(args);
			// set next state

			// read & Assign RecordCode
			int readChar = args.Stream.Read();
			if (readChar == ReaderHelper.iInstance)
			{
				args.CurrentRC = new RecordCode(RecordType.Instance, ExpectSeqNo(args));
				args.State = ReadStateInstanceRecord.Singleton;
			}
			else if (readChar == ReaderHelper.iArray)
			{
				args.CurrentRC = new RecordCode(RecordType.Array, ExpectSeqNo(args));
				args.State = ReadStateArrayRecord.Singleton;
			}
			else if (readChar == ReaderHelper.iTypeDef)
			{
				args.CurrentRC = new RecordCode(RecordType.TypeDef, ExpectSeqNo(args));
				args.State = ReadStateTypeDefRecord.Singleton;
			}
			else
			{
				throw new Error(ErrorCode.UnknownRecordType).AddData(ErrorDataKeys.Actual, readChar);
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
				throw Error.Unexpected(ErrorCode.UnexpectedRecordType, RecordType.TypeDef, refRec.Code.RecType);
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
			if (readChar == ReaderHelper.iBoolTrue)
			{
				vr.Values.Add(true);
				rv.ReadValue(vr, vr.Values.Count, true);
			}
			else if (readChar == ReaderHelper.iBoolFalse)
			{
				vr.Values.Add(false);
				rv.ReadValue(vr, vr.Values.Count, false);
			}
			else if (readChar == ReaderHelper.iStringEncl)
			{
				String str = base.ReadStringStrict(args, false);
				vr.Values.Add(str);
				rv.ReadValue(vr, vr.Values.Count, str);
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
			else if (readChar == ReaderHelper.iDateTimePrefix)
			{
				//base.ReadTill(args, new int[] { ReaderHelper.iFieldSep, ReaderHelper.iRecordSep });
				DateTime dt = new DateTime(
					base.ReadDateTimeDigits(args.Stream, 4),
					base.ReadDateTimeDigits(args.Stream, 2),
					base.ReadDateTimeDigits(args.Stream, 2),
					base.ReadSkipOne(args.Stream).ReadDateTimeDigits(args.Stream, 2),
					base.ReadDateTimeDigits(args.Stream, 2),
					base.ReadDateTimeDigits(args.Stream, 2),
					base.ReadDateTimeDigits(args.Stream, 3)
				);
				vr.Values.Add(dt);
				rv.ReadValue(vr, vr.Values.Count, dt);
			}
			else if(readChar == ReaderHelper.iFieldSep)
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
			else if (readChar >= 0)
			{
				//TODO read number
				base.ReadTill(args, new int[] { ReaderHelper.iFieldSep, ReaderHelper.iRecordSep });
				return;
			}
			if (readChar == -1)
			{
				vr.Values.Add(null);
				rv.ReadValueNull(vr, vr.Values.Count);
				args.State = ReadStateEnd.Singleton;
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
			if (readChar == ReaderHelper.iRefPrefix)
			{
				Record refRec = base.ReadRef(args, false);
				if (refRec.Code.RecType != RecordType.TypeDef)
				{
					throw new Error(ErrorCode.UnexpectedRecordType);
				}
				ArrayRefsRecord rec = new ArrayRefsRecord(args.CurrentRC, (TypeDefRecord)refRec);
				args.SetupRecord(rec);
				args.Read.Read(rec);

				// no need to read extra char as base.readRef has already done that
			}
			else if (readChar == ReaderHelper.iPrimitivePrefix)
			{
				// read one more char to get primitive type
				readChar = args.Stream.Read();
				PrimitiveType pType = ReaderHelper.GetPrimitiveTypeByReadChar(readChar);
				if (pType == PrimitiveType.Unknown)
				{
					throw Error.Unexpected(ErrorCode.UnexpectedChars, Constants.Primitives.Prefix, readChar);
				}

				ArrayPrimitivesRecord arrRec = new ArrayPrimitivesRecord(args.CurrentRC, pType);
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
				throw Error.Unexpected(ErrorCode.UnexpectedChars, Constants.RecordTypeChar.Array, readChar);
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
			TypeDefRecord rec = new TypeDefRecord(args.CurrentRC, base.ReadStringStrict(args));

			// members
			int readChar = 0;
			List<String> members = new List<string>();
			while (true)
			{
				readChar = args.Stream.Read();
				if (readChar == ReaderHelper.iFieldSep)
				{
					members.Add(base.ReadStringStrict(args));
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
					throw Error.Unexpected(ErrorCode.UnexpectedChars, Constants.FieldSeparator, readChar);
				}
			}
		}
	}

	//class ReadStateVersionRecord : ReadStateBase
	//{
	//	public static readonly ReadStateVersionRecord Singleton = new ReadStateVersionRecord();

	//	private ReadStateVersionRecord() : base(ReadStateEnum.VersionRecord) { }

	//	public override void Read(ReadArgs args)
	//	{
	//		base.ReadNonVersionRecordCode(args);
	//		if (args.CurrentRC.RecType != RecordType.Version)
	//		{
	//			throw new Error(ErrorCode.UnexpectedRecordType);
	//		}
	//		VersionRecord vr = new VersionRecord(args.CurrentRC, base.ReadStringField(args));
	//		args.dcRecords[vr.Code.SequenceNo] = vr;
	//		args.Read.Read(vr);

	//		int readChar = args.Stream.Read();
	//		ReadExpectNewRecord(args, readChar);
	//	}
	//}

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
		public static readonly int iDigit0 = Convert.ToInt32('0');
		//public static readonly long[] iDigitsLong =
		//{
		//		Convert.ToInt32('0'),
		//		Convert.ToInt32('1'),
		//		Convert.ToInt32('2'),
		//		Convert.ToInt32('3'),
		//		Convert.ToInt32('4'),
		//		Convert.ToInt32('5'),
		//		Convert.ToInt32('6'),
		//		Convert.ToInt32('7'),
		//		Convert.ToInt32('8'),
		//		Convert.ToInt32('9')
		//};

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
		public static readonly int iPrimBool = Convert.ToInt32(Constants.Primitives.Bool);
		public static readonly int iPrimDateTime = Convert.ToInt32(Constants.Primitives.DateTime);
		public static readonly int iPrimReal = Convert.ToInt32(Constants.Primitives.Real);
		public static readonly int iPrimLong = Convert.ToInt32(Constants.Primitives.Integer);
		public static readonly int iPrimString = Convert.ToInt32(Constants.Primitives.String);

		public static PrimitiveType GetPrimitiveTypeByReadChar(int readChar)
		{
			return (readChar == iPrimBool) ? PrimitiveType.Bool :
				(readChar == iPrimDateTime) ? PrimitiveType.DateTime :
				(readChar == iPrimReal) ? PrimitiveType.Real :
				(readChar == iPrimLong) ? PrimitiveType.Int :
				(readChar == iPrimString) ? PrimitiveType.String :
				PrimitiveType.Unknown;
		}

		public static RecordType ResolveRecordType(int charInt)
		{
			return (charInt == iInstance) ? RecordType.Instance :
				(charInt == iArray) ? RecordType.Array :
				(charInt == iTypeDef) ? RecordType.TypeDef :
				RecordType.Unknown;
		}
	}
}
