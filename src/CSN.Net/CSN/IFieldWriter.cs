using System;
using System.Collections.Generic;
using System.Text;

namespace Abstraction.Csn
{
    public interface IFieldWriter
    {
		IFieldWriter W(string value);

		IFieldWriter W(bool value);

		IFieldWriter W(DateTime value);

		IFieldWriter W(long value);

		IFieldWriter W(double value);

		IFieldWriter W(RecordCode value);

		RecordCode Current { get; }
	}
}
