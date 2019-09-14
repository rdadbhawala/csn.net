﻿using System;
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

		int strPos = 0;
		int strLen = 1000;
		char[] strChars = new char[1000];

		public void StrReset()
		{
			strPos = 0;
		}

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
			return new string(strChars, 0, strPos);
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
				else
				{
					if (readChar == ReaderHelper.iDecimal)
					{
						isDecimal = true;
					}
					else if (readChar == ReaderHelper.iFieldSep)
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

			ValueRecord vr = args.ValueRec;

			if (!isDecimal)
			{
				valueLong *= multiply;
				vr.Values.Add(valueLong);
				args.Read.GetReadValue().ReadValue(vr, vr.Values.Count, valueLong);
			}
			else
			{
				// get a double value
				long decimalValue = 0;
				double factor = 1;

				do
				{
					readChar = args.Stream.Read();
					int digitValue = readChar - ReaderHelper.iDigit0;
					if (digitValue >= 0 && digitValue < 10)
					{
						decimalValue = (decimalValue * 10) + digitValue;
						factor *= 10;
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
				} while (true);

				double realValue = (valueLong + (decimalValue / factor)) * multiply;
				args.Read.GetReadValue().ReadValue(vr, vr.Values.Count, realValue);
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

			args.StrReset();
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
						//sb.Append(Constants.StringEscapeChar);
						args.StrAppend(Constants.StringEscapeChar);
					}
					else if (readChar == ReaderHelper.iStringEncl)
					{
						//sb.Append(Constants.StringFieldEncloser);
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
					//sb.Append(Convert.ToChar(readChar));
					args.StrAppend((char)readChar);
				}
				else
				{
					throw new Error(ErrorCode.UnexpectedEOF);
				}
			}
			return args.StrGet();
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
				throw Error.UnexpectedChars(Constants.FieldSeparator, (char)readChar);
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
