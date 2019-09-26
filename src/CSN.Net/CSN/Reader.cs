using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Csn
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
		readonly StreamReader sReader = null;
		public IRead Read = null;
		public ReaderBase State = ReaderVersion.Singleton;

		public long CurrentSeqNo = -1;
		public ValueRecord ValueRec = null;
		public Dictionary<long, Record> dcRecords = new Dictionary<long, Record>();

		public ReadArgs(StreamReader pStream, IRead pRead)
		{
			this.sReader = pStream;
			this.Read = pRead;
		}

		public void SetupRecord(Record rec)
		{
			dcRecords[rec.SequenceNo] = rec;
			this.ValueRec = rec as ValueRecord;
		}

		#region stream buffer

		const int readMax = 1024;
		readonly char[] readBlock = new char[readMax];
		int readPos = 0;
		int readLen = 0;

		public int ReadOne()
		{
			if (readPos < readLen)
			{
				return readBlock[readPos++];
			}
			else if (readPos == readLen)
			{
				readPos = 0;
				readLen = this.sReader.Read(readBlock, 0, readMax);
				if (readLen == 0)
				{
					readPos = 1;
					return -1;
				}
				else
				{
					return readBlock[readPos++];
				}
			}
			else
			{
				return -1;
			}
		}

		#endregion

		#region String Fields

		int strPos = 0;
		int strLen = 1000;
		char[] strChars = new char[1000];

		public void StrAppend(char readChar)
		{
			if (strPos == strLen)
			{
				char[] newCharArr = new char[strPos + 1000];
				Array.Copy(strChars, newCharArr, strPos);
				strChars = newCharArr;
				strLen += 1000;
			}
			strChars[strPos] = readChar;
			strPos++;
		}

		public String StrGet()
		{
			int len = strPos;
			strPos = 0;
			return new string(strChars, 0, len);
		}

		public void StrReset()
		{
			strPos = 0;
		}

		#endregion
	}


	abstract class ReaderBase : Constants
	{
		public const int iDigit0 = '0';
		public const int iVersion = Constants.RecordTypeChar.Version;
		public const int iTypeDef = Constants.RecordTypeChar.TypeDef;
		public const int iArray = Constants.RecordTypeChar.Array;
		public const int iInstance = Constants.RecordTypeChar.Instance;
		public const int iFieldSep = Constants.FieldSeparator;
		public const int iRecordSep = Constants.RecordSeparator;
		public const int iStringEncl = Constants.StringFieldEncloser;
		public const int iStringEsc = Constants.StringEscapeChar;
		public const int iRefPrefix = Constants.ReferencePrefix;
		public const int iBoolTrue = Constants.BoolTrue;
		public const int iBoolFalse = Constants.BoolFalse;
		public const int iDateTimePrefix = Constants.DateTimePrefix;
		public const int iPrimitivePrefix = Constants.Primitives.Prefix;
		public const int iPrimBool = Constants.Primitives.Bool;
		public const int iPrimDateTime = Constants.Primitives.DateTime;
		public const int iPrimReal = Constants.Primitives.Real;
		public const int iPrimLong = Constants.Primitives.Integer;
		public const int iPrimString = Constants.Primitives.String;
		public const int iDateTimeT = Constants.DateTimeT;
		public const int iMinus = '-';
		public const int iDecimal = '.';

		abstract public void Read(ReadArgs args);

		protected PrimitiveType GetPrimitiveTypeByReadChar(int readChar)
		{
			return (readChar == iPrimBool) ? PrimitiveType.Bool :
				(readChar == iPrimDateTime) ? PrimitiveType.DateTime :
				(readChar == iPrimReal) ? PrimitiveType.Real :
				(readChar == iPrimLong) ? PrimitiveType.Int :
				(readChar == iPrimString) ? PrimitiveType.String :
				PrimitiveType.Unknown;
		}

		protected String ReadStringStrict(ReadArgs args, bool expectOpenEncl = true)
		{
			int readChar = 0;
			// opening encloser
			if (expectOpenEncl)
			{
				readChar = args.ReadOne();
				if (readChar != iStringEncl)
				{
					throw Error.UnexpectedChars(Constants.StringFieldEncloser, Convert.ToChar(readChar));
				}
			}

			while (true)
			{
				readChar = args.ReadOne();
				if (readChar == iStringEncl)
				{
					break;
				}
				else if (readChar == iStringEsc)
				{
					readChar = args.ReadOne();
					if (readChar == iStringEsc)
					{
						args.StrAppend(Constants.StringEscapeChar);
					}
					else if (readChar == iStringEncl)
					{
						args.StrAppend(Constants.StringFieldEncloser);
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
					args.StrAppend((char)readChar);
				}
				else
				{
					throw new Error(ErrorCode.UnexpectedEOF);
				}
			}
			return args.StrGet();
		}

		protected Record ReadRef(ReadArgs args, bool checkFirstChar = true)
		{
			int readChar = 0;
			if (checkFirstChar)
			{
				readChar = args.ReadOne();
				if (readChar != iRefPrefix)
				{
					throw Error.Unexpected(ErrorCode.UnexpectedChars, Constants.ReferencePrefix, readChar);
				}
			}

			long seqNo = 0;
			long digitValue = 0;
			while (true)
			{
				readChar = args.ReadOne();
				digitValue = readChar - iDigit0;
				if (digitValue >= 0 && digitValue < 10)
				{
					seqNo = (seqNo * 10) + digitValue;
				}
				else if (readChar == iFieldSep)
				{
					args.State = ReaderField.Singleton;
					break;
				}
				else if (readChar == iRecordSep)
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
					throw Error.Unexpected(ErrorCode.UnexpectedChars, readChar, digitValue);
				}
			}

			return args.dcRecords[seqNo];
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

			int readChar = args.ReadOne();

			switch (readChar)
			{
				case iBoolTrue:
					vr.Values.Add(true);
					rv.ReadValue(vr, vr.Values.Count, true);
					break;
				case iBoolFalse:
					vr.Values.Add(false);
					rv.ReadValue(vr, vr.Values.Count, false);
					break;
				case iStringEncl:
					String str = base.ReadStringStrict(args, false);
					vr.Values.Add(str);
					rv.ReadValue(vr, vr.Values.Count, str);
					break;
				case iRefPrefix:
					Record rc = base.ReadRef(args, false);
					if (rc.RecType != RecordType.Instance && rc.RecType != RecordType.Array)
					{
						throw Error.UnexpectedRecordType(RecordType.Instance, rc.RecType);
					}
					vr.Values.Add(rc);
					rv.ReadValue(vr, vr.Values.Count, rc);
					return;
				case iDateTimePrefix:
					//base.ReadTill(args, new int[] { iFieldSep, iRecordSep });
					DateTime dt = new DateTime(
						ReadDateTimeDigits(args, 4),
						ReadDateTimeDigits(args, 2),
						ReadDateTimeDigits(args, 2),
						ReadSkipOne(args, iDateTimeT).ReadDateTimeDigits(args, 2),
						ReadDateTimeDigits(args, 2),
						ReadDateTimeDigits(args, 2),
						ReadDateTimeDigits(args, 3)
					);
					vr.Values.Add(dt);
					rv.ReadValue(vr, vr.Values.Count, dt);
					break;
				case iFieldSep:
					vr.Values.Add(null);
					rv.ReadValueNull(vr, vr.Values.Count);
					return;
				case iRecordSep:
					vr.Values.Add(null);
					rv.ReadValueNull(vr, vr.Values.Count);
					args.State = ReaderNewRecord.Singleton;
					return;
				case -1:
					vr.Values.Add(null);
					rv.ReadValueNull(vr, vr.Values.Count);
					args.State = ReaderEnd.Singleton;
					return;
				default:
					ReadNumber(args, readChar);
					return;
			}

			// fixed width values (bool, datetime) or values with delimiter (string)
			// still have one extra character to read in order to determine next step
			readChar = args.ReadOne();
			if (readChar == iFieldSep)
			{
				args.State = ReaderField.Singleton;
			}
			else if (readChar == iRecordSep)
			{
				args.State = ReaderNewRecord.Singleton;
			}
			else if (readChar == -1)
			{
				args.State = ReaderEnd.Singleton;
			}
			else
			{
				throw Error.UnexpectedChars(Constants.FieldSeparator, (char)readChar);
			}
		}

		protected void ReadNumber(ReadArgs args, int readChar)
		{
			long valueLong = 0;
			long multiply = 1;
			bool isDecimal = false;
			if (readChar == iMinus)
			{
				multiply = -1;
				args.StrAppend((char)readChar);

				readChar = args.ReadOne();
			}

			do
			{
				int digitValue = readChar - iDigit0;
				if (digitValue >= 0 && digitValue < 10)
				{
					valueLong = (valueLong * 10) + digitValue;
					args.StrAppend((char)readChar);
				}
				else
				{
					if (readChar == iDecimal)
					{
						args.StrAppend((char)readChar);
						isDecimal = true;
					}
					else if (readChar == iFieldSep)
					{
						args.State = ReaderField.Singleton;
					}
					else if (readChar == iRecordSep)
					{
						args.State = ReaderNewRecord.Singleton;
					}
					else if (readChar == -1)
					{
						args.State = ReaderEnd.Singleton;
					}
					else
					{
						throw Error.Unexpected(ErrorCode.UnexpectedChars, iDigit0, readChar);
					}
					break;
				}
				readChar = args.ReadOne();
			} while (true);

			ValueRecord vr = args.ValueRec;

			if (!isDecimal)
			{
				valueLong *= multiply;
				vr.Values.Add(valueLong);
				args.Read.GetReadValue().ReadValue(vr, vr.Values.Count, valueLong);
				args.StrReset();
			}
			else
			{
				// get a double value
				do
				{
					readChar = args.ReadOne();
					int digitValue = readChar - iDigit0;
					if (digitValue >= 0 && digitValue < 10)
					{
						args.StrAppend((char)readChar);
					}
					else
					{
						if (readChar == iFieldSep)
						{
							args.State = ReaderField.Singleton;
						}
						else if (readChar == iRecordSep)
						{
							args.State = ReaderNewRecord.Singleton;
						}
						else if (readChar == -1)
						{
							args.State = ReaderEnd.Singleton;
						}
						else
						{
							throw Error.Unexpected(ErrorCode.UnexpectedChars, iDigit0, readChar);
						}
						break;
					}
				} while (true);

				double realValue = double.Parse(args.StrGet());
				args.ValueRec.Values.Add(realValue);
				args.Read.GetReadValue().ReadValue(vr, vr.Values.Count, realValue);
			}
		}

		protected ReaderField ReadSkipOne(ReadArgs args, int expectedChar)
		{
			int readChar = args.ReadOne();
			if (readChar != expectedChar)
			{
				throw Error.Unexpected(ErrorCode.UnexpectedChars, expectedChar, readChar);
			}
			return this;
		}

		public int ReadDateTimeDigits(ReadArgs args, int len)
		{
			int value = 0;
			int mapValue = 0;
			for (int ctr = 0; ctr < len; ctr++)
			{
				int readChar = args.ReadOne();
				mapValue = readChar - iDigit0;
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
	}

	class ReaderEnd : ReaderBase
	{
		public static readonly ReaderEnd Singleton = new ReaderEnd();

		private ReaderEnd() { }

		// ideally, according to the design of the where loop,
		// this method should not get triggered
		public override void Read(ReadArgs args)
		{
			throw new Error(ErrorCode.NothingToRead);
		}
	}
}
