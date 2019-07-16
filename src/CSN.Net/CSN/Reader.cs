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
		readonly StreamReader sr = null;

		public Reader(StreamReader reader)
		{
			this.sr = reader;
		}

		public void Read(IRead callback)
		{
			ReadArgs args = new ReadArgs(this.sr, callback);
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
		WhatNext
	}

	class ReadArgs
	{
		// parser vars
		public StreamReader Stream = null;
		public IRead Read = null;
		public ReadStateBase State = ReadStateVersionRecord.Singleton;

		// state vars
		public RecordCode CurrentRC = null;

		public ReadArgs(StreamReader pStream, IRead pRead)
		{
			this.Stream = pStream;
			this.Read = pRead;
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

		protected IEnumerable<int> ReadTill(ReadArgs args, int tillChar, bool includeTillChar = false)
		{
			LinkedList<int> charInts = new LinkedList<int>();

			int readChar = -1;
			while ((readChar = args.Stream.Read()) != -1)
			{
				if (readChar != tillChar || includeTillChar)
				{
					charInts.AddLast(readChar);
				}
				if (readChar == tillChar)
				{
					break;
				}
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
						throw new Error(ErrorCode.NotEscapeChar).AddData(ErrorDataKeys.ActualChar, Convert.ToChar(readChar));
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

					long seqNo = 0;
					long mapValue = 0;
					foreach (int digit in this.ReadTill(args, ReaderHelper.iFieldSep))
					{
						if (ReaderHelper.DigitMap.TryGetValue(digit, out mapValue))
						{
							seqNo = (seqNo * 10) + mapValue;
						}
						else
						{
							throw Error.UnexpectedChars('0', Convert.ToChar(mapValue));
						}
					}
					args.CurrentRC = new RecordCode(ReaderHelper.ResolveRecordType(readChar), seqNo);

					break;
				default:
					throw new Error(ErrorCode.UnknownRecordType);
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
			ReadRecordCode(args);
			// set next state
			switch (args.CurrentRC.RecType)
			{
				//case RecordType.Instance: args.State = new ReadStateInstanceRecord(); break;
				case RecordType.Array: args.State = new ReadStateArrayRecord(); break;
				case RecordType.TypeDef: args.State = ReadStateTypeDefRecord.Singleton; break;
				default:
					throw new Error(ErrorCode.UnexpectedRecordType);
			}

		}
	}

	class ReadStateArrayRecord : ReadStateBase
	{
		public static readonly ReadStateArrayRecord Singleton = new ReadStateArrayRecord();

		private ReadStateArrayRecord() : base(ReadStateEnum.ArrayRecord) { }

		public override void Read(ReadArgs args)
		{
			throw new NotImplementedException();
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
					args.Read.Read(rec);
					args.State = ReadStateNewRecord.Singleton;
					break;
				}
				else
				{
					throw Error.UnexpectedChars(Constants.DefaultFieldSeparator, Convert.ToChar(readChar));
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
			args.Read.Read(new VersionRecord(args.CurrentRC, base.ReadStringField(args)));

			args.State = ReadStateWhatNext.Singleton;
		}
	}

	class ReadStateWhatNext : ReadStateBase
	{
		public static readonly ReadStateWhatNext Singleton = new ReadStateWhatNext();
		private ReadStateWhatNext() : base(ReadStateEnum.WhatNext) { }

		public override void Read(ReadArgs args)
		{
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
				throw Error.UnexpectedChars(Constants.DefaultRecordSeparator, Convert.ToChar(readChar));
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
		public static readonly int iVersion = Convert.ToInt32(Constants.RecordTypeChar.Version);
		public static readonly int iTypeDef = Convert.ToInt32(Constants.RecordTypeChar.TypeDef);
		public static readonly int iArray = Convert.ToInt32(Constants.RecordTypeChar.Array);
		public static readonly int iInstance = Convert.ToInt32(Constants.RecordTypeChar.Instance);
		public static readonly int iFieldSep = Convert.ToInt32(Constants.DefaultFieldSeparator);
		public static readonly int iRecordSep = Convert.ToInt32(Constants.DefaultRecordSeparator);
		public static readonly int iStringEncl = Convert.ToInt32(Constants.StringFieldEncloser);
		public static readonly int iStringEsc = Convert.ToInt32(Constants.StringEscapeChar);

		public static readonly Dictionary<int, long> DigitMap = null;

		static ReaderHelper()
		{
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
