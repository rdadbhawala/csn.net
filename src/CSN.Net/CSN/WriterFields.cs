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
			FieldDateTime.F.WriteField(this.sw, value);
			return this;
		}

		public IWriterField W(long value)
		{
			sw.Write(Constants.FieldSeparator);
			sw.Write(value);
			return this;
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
			FieldDouble.F.WriteField(this.sw, value);
			return this;
		}

		public IWriterField W(RecordCode value)
		{
			if (value == null)
			{
				sw.Write(Constants.FieldSeparator);
			}
			else
			{
				sw.Write(Constants.FieldSeparator);
				sw.Write(Constants.ReferencePrefix);
				sw.Write(value.SequenceNo);
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
