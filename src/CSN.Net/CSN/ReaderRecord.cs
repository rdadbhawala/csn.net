using System;
using System.Collections.Generic;
using System.Text;

namespace Csn
{
	class ReaderVersion : ReaderBase
	{
		public static readonly ReaderVersion Singleton = new ReaderVersion();
		private ReaderVersion() { }

		public override void Read(ReadArgs args)
		{
			// read Version Record Code '0,'
			if (args.ReadOne() != iDigit0 || args.ReadOne() != iFieldSep)
			{
				throw Error.UnexpectedRecordType(RecordType.Version, RecordType.Unknown);
			}
			args.CurrentSeqNo = 0;

			// TODO - version validation ??

			VersionRecord vr = new VersionRecord(base.ReadStringStrict(args));
			args.dcRecords[args.CurrentSeqNo] = vr;
			args.Read.Read(vr);

			int readChar = args.ReadOne();
			if (readChar == iRecordSep)
			{
				args.State = ReaderNewRecord.Singleton;
			}
			else if (readChar == -1)
			{
				args.State = ReaderEnd.Singleton;
			}
			else
			{
				throw Error.Unexpected(ErrorCode.UnexpectedChars, Constants.RecordSeparator, readChar);
			}
		}
	}

	// except version record
	class ReaderNewRecord : ReaderBase
	{
		public static readonly ReaderNewRecord Singleton = new ReaderNewRecord();
		private ReaderNewRecord() { }

		public override void Read(ReadArgs args)
		{
			args.CurrentSeqNo = ExpectNextSeqNo(args);

			int readChar = args.ReadOne();
			if (readChar == iRefPrefix)
			{
				args.State = ReaderInstance.Singleton;
			}
			else if (readChar == iArray)
			{
				readChar = args.ReadOne();
				if (readChar != iFieldSep)
				{
					throw Error.Unexpected(ErrorCode.UnexpectedChars, FieldSeparator, readChar);
				}

				ArrayRecord rec = new ArrayRecord(args.CurrentSeqNo);
				args.SetupRecord(rec);
				args.Read.Read(rec);

				args.State = ReaderField.Singleton;
			}
			else if (readChar == iStringEncl)
			{
				args.State = ReaderTypeDef.Singleton;
			}
			else
			{
				throw new Error(ErrorCode.UnknownRecordType).AddData(ErrorDataKeys.Actual, readChar);
			}
		}

		private long ExpectNextSeqNo(ReadArgs args)
		{
			long actualSeqNo = 0;
			while (true)
			{
				int readChar = args.ReadOne();
				int digit = readChar - iDigit0;
				if (digit >= 0 && digit < 10)
				{
					actualSeqNo = (actualSeqNo * 10) + digit;
				}
				else
				{
					if (readChar == iFieldSep)
					{
						break;
					}
					else
					{
						throw Error.Unexpected(ErrorCode.UnexpectedChars, '0', readChar);
					}
				}
			}

			if (actualSeqNo != args.dcRecords.Count)
			{
				throw Error.Unexpected(ErrorCode.UnexpectedSequenceNo, args.dcRecords.Count, actualSeqNo);
			}

			return actualSeqNo;
		}

		// for seq no > 0 in the Record Code only
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
				readChar = args.ReadOne();
				expectedSeqNo = stkSeqNo.Pop();
				if ((expectedSeqNo + iDigit0) != readChar)
				{
					throw Error.UnexpectedChars('0', Convert.ToChar(readChar));
				}
			}

			// read the fieldsep
			if ((readChar = args.ReadOne()) != iFieldSep)
			{
				throw Error.UnexpectedChars(Constants.FieldSeparator, Convert.ToChar(readChar));
			}

			return args.dcRecords.Count;
		}

	}

	class ReaderTypeDef : ReaderBase
	{
		public static readonly ReaderTypeDef Singleton = new ReaderTypeDef();
		private ReaderTypeDef() { }

		public override void Read(ReadArgs args)
		{
			// sequence no, opening quote of type name has been read,
			TypeDefRecord rec = new TypeDefRecord(args.CurrentSeqNo, base.ReadStringStrict(args, false), ReadMembers(args));
			args.SetupRecord(rec);
			args.Read.Read(rec);

			//ReadMembers(args, rec);
		}

		private string[] ReadMembers(ReadArgs args)
		{
			// members
			int readChar = 0;
			List<String> members = new List<string>();
			while (true)
			{
				readChar = args.ReadOne();
				if (readChar == iFieldSep)
				{
					members.Add(base.ReadStringStrict(args));
				}
				else if (readChar == iRecordSep)
				{
					args.State = ReaderNewRecord.Singleton;
					return members.ToArray();
				}
				else
				{
					throw Error.Unexpected(ErrorCode.UnexpectedChars, Constants.FieldSeparator, readChar);
				}
			}
		}
	}

	class ReaderInstance : ReaderBase
	{
		public static readonly ReaderInstance Singleton = new ReaderInstance();

		private ReaderInstance() { }

		public override void Read(ReadArgs args)
		{
			Record refRec = base.ReadRef(args, false);
			if (refRec.RecType != RecordType.TypeDef)
			{
				throw Error.Unexpected(ErrorCode.UnexpectedRecordType, RecordType.TypeDef, refRec.RecType);
			}

			TypeDefRecord refType = refRec as TypeDefRecord;
			InstanceRecord rec = new InstanceRecord(args.CurrentSeqNo, refType);
			args.SetupRecord(rec);
			args.Read.Read(rec);

			// base.ReadRef must have already set the next state
		}
	}
}
