using System;
using System.Collections.Generic;
using System.Text;

namespace Csn
{
    public interface IWriterField
    {
		IWriterField W(bool value);
		IWriterField W(bool[] values);

		IWriterField W(string value);
		IWriterField W(string[] values);

		IWriterField W(DateTime value);
		IWriterField W(DateTime[] values);

		IWriterField W(long value);
		IWriterField W(long[] values);

		IWriterField W(double value);
		IWriterField W(double[] values);

		IWriterField WRef(long? value);
		IWriterField WRef(long?[] values);

		IWriterField WNull();

		long Current { get; }
	}
}
