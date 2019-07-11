using Abstraction.Csn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FiguringItOut
{
    class Sample
    {
		public static void PrintCsn()
		{
			// externals
			Stream mstream = Console.OpenStandardOutput();
			StreamWriter sWriter = new StreamWriter(mstream);

			// setup the CSN Parser

			Writer csnw = new Writer(sWriter);

			RecordCode tRef = csnw.WriteTypeDefRecord("AllPrimitives", "BooleanTrue", "BooleanFalse", "DateTime", "String", "Real", "Integer").Current;
			RecordCode iRef = csnw.WriteInstanceRecord(tRef, true, false, new DateTime(2019, 03, 12, 19, 24, 33, 567, DateTimeKind.Utc), "Label", -123.45, 345).Current;
			sWriter.Flush();
		}
	}
}
