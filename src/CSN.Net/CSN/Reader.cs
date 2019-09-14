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
			} while (args.State != ReaderEnd.Singleton);
		}
	}

	class ReadArgs
	{
		// parser vars
		public StreamReader Stream = null;
		public IRead Read = null;
		public ReaderBase State = ReaderVersion.Singleton;

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

	abstract class ReaderBase
	{
		protected ReaderBase() { }

		abstract public void Read(ReadArgs args);

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
				args.State = ReaderField.Singleton;
			}
			else if (readChar == ReaderHelper.iRecordSep)
			{
				args.State = ReaderNewRecord.Singleton;
			}
			else if (readChar == -1)
			{
				args.State = ReaderEnd.Singleton;
			}

			return charInts;
		}

		protected void ReadNumber(ReadArgs args, int readChar)
		{
			long valueLong = 0;
			long multiply = 1;
			bool isDecimal = false;
			if (readChar == ReaderHelper.iMinus)
			{
				multiply = -1;
				readChar = args.Stream.Read();
			}

			do
			{
				int digitValue = readChar - ReaderHelper.iDigit0;
				if (digitValue >= 0 && digitValue < 10)
				{
					valueLong = (valueLong * 10) + digitValue;
				}
				else if (readChar == ReaderHelper.iDecimal)
				{
					isDecimal = true;
					break;
				}
				else
				{
					if (readChar == ReaderHelper.iFieldSep)
					{
						args.State = ReaderField.Singleton;
					}
					else if (readChar == ReaderHelper.iRecordSep)
					{
						args.State = ReaderNewRecord.Singleton;
					}
					else if (readChar == -1)
					{
						args.State = ReaderEnd.Singleton;
					}
					else
					{
						throw Error.Unexpected(ErrorCode.UnexpectedChars, ReaderHelper.iDigit0, readChar);
					}
					break;
				}
				readChar = args.Stream.Read();
			} while (true);

			if (!isDecimal)
			{
				valueLong *= multiply;
				ValueRecord vr = args.ValueRec;
				vr.Values.Add(valueLong);
				args.Read.GetReadValue().ReadValue(vr, vr.Values.Count, valueLong);
			}
			else
			{
				// get a double value
				ReadTill(args, new int[] { ReaderHelper.iFieldSep, ReaderHelper.iRecordSep });
			}
		}

		// optimized
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

		// optimized
		protected Record ReadRef(ReadArgs args, bool checkFirstChar = true)
		{
			int readChar = 0;
			if (checkFirstChar)
			{
				readChar = args.Stream.Read();
				if (readChar != ReaderHelper.iRefPrefix)
				{
					throw Error.Unexpected(ErrorCode.UnexpectedChars, Constants.ReferencePrefix, readChar);
				}
			}

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
					args.State = ReaderField.Singleton;
					break;
				}
				else if (readChar == ReaderHelper.iRecordSep)
				{
					args.State = ReaderNewRecord.Singleton;
					break;
				}
				else if (readChar == -1)
				{
					args.State = ReaderEnd.Singleton;
					break;
				}
				else
				{
					throw Error.Unexpected(ErrorCode.UnexpectedChars, readChar, mapValue);
				}
			}

			return args.dcRecords[seqNo];
		}

		// optimized // for seq no > 0 in the Record Code only
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

		// optimized
		protected ReaderBase ReadSkipOne(StreamReader stream, int expectedChar)
		{
			int readChar = stream.Read();
			if (readChar != expectedChar)
			{
				throw Error.Unexpected(ErrorCode.UnexpectedChars, expectedChar, readChar);
			}
			return this;
		}

		//protected ReadStateBase ReadSkip(StreamReader stream, int skipInitial)
		//{
		//	if (stream.Read(new char[skipInitial], 0, skipInitial) != skipInitial)
		//	{
		//		throw new Error(ErrorCode.UnexpectedEOF);
		//	}
		//	return this;
		//}

		// optimized
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

		// optimized
		protected void ReadExpectNewRecord(ReadArgs args, int readChar)
		{
			if (readChar == ReaderHelper.iRecordSep)
			{
				args.State = ReaderNewRecord.Singleton;
			}
			else if (readChar == -1)
			{
				args.State = ReaderEnd.Singleton;
			}
			else
			{
				throw Error.UnexpectedChars(Constants.RecordSeparator, Convert.ToChar(readChar));
			}
		}
	}

	class ReaderField : ReaderBase
	{
		public static readonly ReaderField Singleton = new ReaderField();
		private ReaderField() { }

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
					base.ReadSkipOne(args.Stream, ReaderHelper.iDateTimeT).ReadDateTimeDigits(args.Stream, 2),
					base.ReadDateTimeDigits(args.Stream, 2),
					base.ReadDateTimeDigits(args.Stream, 2),
					base.ReadDateTimeDigits(args.Stream, 3)
				);
				vr.Values.Add(dt);
				rv.ReadValue(vr, vr.Values.Count, dt);
			}
			else if (readChar == ReaderHelper.iFieldSep)
			{
				vr.Values.Add(null);
				rv.ReadValueNull(vr, vr.Values.Count);
				return;
			}
			else if (readChar == ReaderHelper.iRecordSep)
			{
				vr.Values.Add(null);
				rv.ReadValueNull(vr, vr.Values.Count);
				args.State = ReaderNewRecord.Singleton;
				return;
			}
			else if (readChar >= 0)
			{
				//TODO read number
				//base.ReadTill(args, new int[] { ReaderHelper.iFieldSep, ReaderHelper.iRecordSep });
				base.ReadNumber(args, readChar);
				return;
			}
			else if (readChar == -1)
			{
				vr.Values.Add(null);
				rv.ReadValueNull(vr, vr.Values.Count);
				args.State = ReaderEnd.Singleton;
				return;
			}

			// fixed width values (bool, datetime) or values with delimiter (string)
			// still have one extra character to read in order to determine next step
			readChar = args.Stream.Read();
			if (readChar == ReaderHelper.iFieldSep)
			{
				args.State = ReaderField.Singleton;
			}
			else if (readChar == ReaderHelper.iRecordSep)
			{
				args.State = ReaderNewRecord.Singleton;
			}
			else if (readChar == -1)
			{
				args.State = ReaderEnd.Singleton;
			}
			else
			{
				throw Error.UnexpectedChars(Constants.FieldSeparator, Convert.ToChar(readChar));
			}
		}
	}

	class ReaderEnd : ReaderBase
	{
		public static readonly ReaderEnd Singleton = new ReaderEnd();

		private ReaderEnd() { }

		public override void Read(ReadArgs args)
		{
			throw new Error(ErrorCode.NothingToRead);
		}
	}

	class ReaderHelper
	{
		public static readonly int iDigit0 = Convert.ToInt32('0');
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
		public static readonly int iDateTimeT = Convert.ToInt32(Constants.DateTimeT);
		public static readonly int iMinus = Convert.ToInt32('-');
		public static readonly int iDecimal = Convert.ToInt32('.');

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
