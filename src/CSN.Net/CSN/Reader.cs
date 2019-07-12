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
		End
	}

	class ReadArgs
	{
		// parser vars
		public StreamReader Stream = null;
		public IRead Read = null;
		public ReadStateBase State = ReadStateNewRecord.Singleton;

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
		public ReadStateBase (ReadStateEnum pState)
		{
			this.ReadState = pState;
		}

		public ReadStateEnum ReadState { get; private set; }

		abstract public void Read(ReadArgs args);

		protected IEnumerable<int> ReadTill(ReadArgs args, int till, bool exclude = true)
		{
			LinkedList<int> charInts = new LinkedList<int>();

			int readChar = -1;
			while ((readChar = args.Stream.Read()) != -1) {
				if (readChar != till || !exclude)
				{
					charInts.AddLast(readChar);
				}
				if (readChar == till)
				{
					break;
				}
			}

			return charInts; //.ToArray();
		}

		protected string ReadStringField(ReadArgs args)
		{
			// opening encloser
			int readChar = args.Stream.Read();
			if (readChar != ReaderHelper.iStringEncl)
			{
				throw Error.UnexpectedChars(Constants.StringFieldEncloser, Convert.ToChar(readChar));
			}

			LinkedList<int> charInts = new 
			while ((readChar = args.Stream.Read()) != -1)
			{
				
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
			// read & Assign RecordCode
			int charInt = args.Stream.Read();
			long value = 0;
			foreach (int digit in base.ReadTill(args, ReaderHelper.iFieldSep))
			{
				value = (value * 10) + ReaderHelper.DigitMap[digit];
			}
			args.CurrentRC = new RecordCode(ReaderHelper.ResolveRecordType(charInt), value);

			// set next state
			switch (args.CurrentRC.RecType)
			{
				//case RecordType.Instance: args.State = new ReadStateInstanceRecord(); break;
				//case RecordType.Array: args.State = new ReadStateArrayRecord(); break;
				//case RecordType.TypeDef: args.State = new ReadStateTypeDefRecord(); break;
				case RecordType.Version: args.State = ReadStateVersionRecord.Singleton; break;
				default: throw new Error(ErrorCode.UnknownRecordType);
			}
		}
	}

	class ReadStateVersionRecord : ReadStateBase
	{
		public static readonly ReadStateVersionRecord Singleton = new ReadStateVersionRecord();

		private ReadStateVersionRecord() : base(ReadStateEnum.VersionRecord) { }

		public override void Read(ReadArgs args)
		{
			String version = base.ReadStringField(args);

			int readChar = args.Stream.Read();
			if (readChar == -1)
			{
				args.State = ReadStateEnd.Singleton;
			} else if (readChar == ReaderHelper.iRecordSep)
			{
				args.State = ReadStateNewRecord.Singleton;
			} else
			{
				throw Error.UnexpectedChars(Constants.DefaultRecordSeparator, Convert.ToChar(readChar));
			}
			args.Read.VersionRecord(args.CurrentRC, version);
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
				(charInt == iTypeDef) ? RecordType.TypeDef : RecordType.Version;
		}
	}
}
