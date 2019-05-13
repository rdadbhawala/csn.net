using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Abstraction.Csn
{
    public interface IValues
    {
		void WriteValues(StreamWriter sw);
		void WriteType(StreamWriter sw);
    }
}
