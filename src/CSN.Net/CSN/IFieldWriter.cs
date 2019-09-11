using System;
using System.Collections.Generic;
using System.Text;

namespace Abstraction.Csn
{
    public interface IFieldWriter
    {
		IFieldWriter W(bool value);

		IFieldWriter W(string value);

		IFieldWriter W(DateTime value);

		IFieldWriter W(long value);

		IFieldWriter W(double value);

		IFieldWriter W(RecordCode value);

		RecordCode Current { get; }

		IFieldWriter W(bool[] values);
	}
}
