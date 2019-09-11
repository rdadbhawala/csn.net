using System;
using System.Collections.Generic;
using System.Text;

namespace Abstraction.Csn
{
    public interface IWriterField
    {
		IWriterField W(bool value);

		IWriterField W(string value);

		IWriterField W(DateTime value);

		IWriterField W(long value);

		IWriterField W(double value);

		IWriterField W(RecordCode value);

		RecordCode Current { get; }

		IWriterField W(bool[] values);

		IWriterField W(long[] values);

		IWriterField W(string[] values);
	}
}
