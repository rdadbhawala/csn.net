using System;
using System.Collections.Generic;
using System.Text;

namespace Abstraction.Csn
{
	class ReaderVersion : ReaderBase
	{
		public static readonly ReaderVersion Singleton = new ReaderVersion();
		private ReaderVersion() { }

		public override void Read(ReadArgs args)
		{
			// read Version Record Code 'V0,'
			if (args.ReadOne() != ReaderHelper.iVersion || args.ReadOne() != ReaderHelper.iDigit0 || args.ReadOne() != ReaderHelper.iFieldSep)
			{
				throw Error.UnexpectedRecordType(RecordType.Version, RecordType.Unknown);
			}
			args.CurrentRC = new RecordCode(RecordType.Version, 0);

			// TODO - version validation ??

			VersionRecord vr = new VersionRecord(args.CurrentRC, base.ReadStringStrict(args));
			args.dcRecords[vr.Code.SequenceNo] = vr;
			args.Read.Read(vr);

			base.ReadExpectNewRecord(args, args.ReadOne());
		}
	}

	// except version record
	class ReaderNewRecord : ReaderBase
	{
		public static readonly ReaderNewRecord Singleton = new ReaderNewRecord();
		private ReaderNewRecord() { }

		public override void Read(ReadArgs args)
		{
			int readChar = args.ReadOne();
			if (readChar == ReaderHelper.iInstance)
			{
				args.CurrentRC = new RecordCode(RecordType.Instance, ExpectSeqNo(args));
				args.State = ReaderInstance.Singleton;
			}
			else if (readChar == ReaderHelper.iArray)
			{
				args.CurrentRC = new RecordCode(RecordType.Array, ExpectSeqNo(args));
				args.State = ReaderArray.Singleton;
			}
			else if (readChar == ReaderHelper.iTypeDef)
			{
				args.CurrentRC = new RecordCode(RecordType.TypeDef, ExpectSeqNo(args));
				args.State = ReaderTypeDef.Singleton;
			}
			else
			{
				throw new Error(ErrorCode.UnknownRecordType).AddData(ErrorDataKeys.Actual, readChar);
			}
		}
	}

	class ReaderTypeDef : ReaderBase
	{
		public static readonly ReaderTypeDef Singleton = new ReaderTypeDef();
		private ReaderTypeDef() { }

		public override void Read(ReadArgs args)
		{
			// record code has been read,
			TypeDefRecord rec = new TypeDefRecord(args.CurrentRC, base.ReadStringStrict(args));

			// members
			int readChar = 0;
			List<String> members = new List<string>();
			while (true)
			{
				readChar = args.ReadOne();
				if (readChar == ReaderHelper.iFieldSep)
				{
					members.Add(base.ReadStringStrict(args));
				}
				else if (readChar == ReaderHelper.iRecordSep)
				{
					rec.Members = members.ToArray();
					args.SetupRecord(rec);
					args.Read.Read(rec);
					args.State = ReaderNewRecord.Singleton;
					break;
				}
				else
				{
					throw Error.Unexpected(ErrorCode.UnexpectedChars, Constants.FieldSeparator, readChar);
				}
			}
		}
	}

	class ReaderArray : ReaderBase
	{
		public static readonly ReaderArray Singleton = new ReaderArray();
		private ReaderArray() { }

		public override void Read(ReadArgs args)
		{
			// read array type: primitive or typeDef
			int readChar = args.ReadOne();
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
				readChar = args.ReadOne();
				PrimitiveType pType = ReaderHelper.GetPrimitiveTypeByReadChar(readChar);
				if (pType == PrimitiveType.Unknown)
				{
					throw Error.Unexpected(ErrorCode.UnexpectedChars, Constants.Primitives.Prefix, readChar);
				}

				ArrayPrimitivesRecord arrRec = new ArrayPrimitivesRecord(args.CurrentRC, pType);
				args.SetupRecord(arrRec);
				args.Read.Read(arrRec);

				// read a field sep
				readChar = args.ReadOne();
				if (readChar == ReaderHelper.iFieldSep)
				{
					args.State = ReaderField.Singleton;
				}
				else if (readChar == -1)
				{
					args.State = ReaderEnd.Singleton;
				}
				else
				{
					throw Error.Unexpected(ErrorCode.UnexpectedChars, Constants.FieldSeparator, readChar);
				}
			}
			else
			{
				throw Error.Unexpected(ErrorCode.UnexpectedChars, Constants.RecordTypeChar.Array, readChar);
			}
		}
	}

	class ReaderInstance : ReaderBase
	{
		public static readonly ReaderInstance Singleton = new ReaderInstance();

		private ReaderInstance() { }

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
}
