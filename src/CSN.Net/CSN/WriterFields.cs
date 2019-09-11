using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Abstraction.Csn
{
    public partial class Writer
    {
		protected readonly StreamWriter sw = null;

		public RecordCode Current { get; protected set; }

		public IWriterField W(string value)
		{
			sw.Write(Constants.FieldSeparator);
			if (value != null)
			{
				sw.Write(Constants.StringFieldEncloser);

				int charsLen = value.Length;
				if (charsLen > 0)
				{
					char[] chars = value.ToCharArray();
					for (int charCtr = 0; charCtr < charsLen; charCtr++)
					{
						char v = chars[charCtr];
						if (v == Constants.StringFieldEncloser || v == Constants.StringEscapeChar)
						{
							sw.Write(Constants.StringEscapeChar);
						}

						sw.Write(v);
					}
				}

				sw.Write(Constants.StringFieldEncloser);
			}
			return this;
		}

		public IWriterField W(string[] values)
		{
			for (int i = 0, j = values.Length; i < j; i++)
			{
				this.W(values[i]);
			}
			return this;
		}

		public IWriterField W(bool value)
		{
			sw.Write(Constants.FieldSeparator);
			sw.Write(value ? Constants.BoolTrue : Constants.BoolFalse);
			return this;
		}

		public IWriterField W(bool[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				W(values[i]);
			}
			return this;
		}

		public IWriterField W(DateTime value)
		{
			sw.Write(Constants.FieldSeparator);

			char[] arrDt = { 'D', '0', '0', '0', '0', '0', '0', '0', '0', 'T', '0', '0', '0', '0', '0', '0', '0', '0', '0' };
			WriteIntToCharArr(arrDt, 1, 4, value.Year);
			WriteIntToCharArr(arrDt, 5, 2, value.Month);
			WriteIntToCharArr(arrDt, 7, 2, value.Day);
			WriteIntToCharArr(arrDt, 10, 2, value.Hour);
			WriteIntToCharArr(arrDt, 12, 2, value.Minute);
			WriteIntToCharArr(arrDt, 14, 2, value.Second);
			WriteIntToCharArr(arrDt, 16, 3, value.Millisecond);
			sw.Write(arrDt, 0, 19);

			return this;
		}

		private void WriteIntToCharArr(char[] arrDt, int index, int len, int value)
		{
			for (int i = index + len - 1; i >= index && value > 0; i--)
			{
				arrDt[i] = chDigits[value % 10];
				value /= 10;
			}
		}

		private static readonly char[] chDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
		private static readonly char chMinus = '-';
		private const int longArrLen = 20;

		public IWriterField W(long value)
		{
			sw.Write(Constants.FieldSeparator);
			WriteLongRaw(value);
			return this;
		}

		private void WriteLongRaw(long value)
		{
			if (value < 0)
			{
				sw.Write(chMinus);
				value = -value;
			}
			// no need to initialize the chValue array as only updated array items are printed
			char[] chValue = new char[longArrLen];
			int pos = chValue.Length - 1;
			do
			{
				chValue[pos] = chDigits[value % 10];
				value /= 10;
				pos--;
			} while (value > 0);
			pos++;
			sw.Write(chValue, pos, longArrLen - pos);
		}

		public IWriterField W(long[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				W(values[i]);
			}
			return this;
		}

		public IWriterField W(double value)
		{
			sw.Write(Constants.FieldSeparator);
			sw.Write(value);
			return this;
		}

		public IWriterField W(double[] values)
		{
			for (int i = 0, j = values.Length; i < j; i++)
			{
				W(values[i]);
			}
			return this;
		}

		public IWriterField W(RecordCode value)
		{
			if (value == null)
			{
				WNull();
			}
			else
			{
				sw.Write(Constants.FieldSeparator);
				sw.Write(Constants.ReferencePrefix);
				WriteLongRaw(value.SequenceNo);
			}
			return this;
		}

		public IWriterField W(RecordCode[] values)
		{
			for (int i = 0, j = values.Length; i < j; i++)
			{
				W(values[i]);
			}
			return this;
		}

		public IWriterField WNull()
		{
			sw.Write(Constants.FieldSeparator);
			return this;
		}
	}
}
