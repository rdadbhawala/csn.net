using Csn;
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

			long tRef = csnw.WriteTypeDef("AllPrimitives", "BooleanTrue", "BooleanFalse", "DateTime", "String", "Real", "Integer").Current;
			long iRef = csnw.WriteInstance(tRef).W(true).W(false).W(new DateTime(2019, 03, 12, 19, 24, 33, 567, DateTimeKind.Utc)).W("Label").W(-123.45).W(345).Current;
			sWriter.Flush();
		}
	}
}
