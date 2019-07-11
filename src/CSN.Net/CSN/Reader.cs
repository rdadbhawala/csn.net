using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Abstraction.Csn
{
    public class Reader
		: IReader
    {
		StreamReader sr = null;

		public Reader(StreamReader reader)
		{
			this.sr = reader;
		}
    }
}
