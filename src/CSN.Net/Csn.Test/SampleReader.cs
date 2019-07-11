using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Abstraction.Csn.Test
{
	[TestClass]
	public class SampleReader
	{
		[TestMethod]
		public void Test()
		{
			// externals
			MemoryStream mstream = new MemoryStream();
			StreamWriter sw = new StreamWriter(mstream);
			sw.Write(TestResources.AllPrimitives);
			sw.Flush();
			mstream.Position = 0;
			StreamReader sr = new StreamReader(mstream);

			IReader rdr = new Reader(sr);


			//RecordCode tRef = csnw.WriteTypeDefRecord("AllPrimitives", "BooleanTrue", "BooleanFalse", "DateTime", "String", "Real", "Integer").Current;
			//Assert.AreEqual(1, tRef.SequenceNo, "Ref Type - All Primitives");

			//RecordCode iRef = csnw.WriteInstanceRecord(tRef, true, false, new DateTime(2019, 03, 12, 19, 24, 33, 567, DateTimeKind.Utc), "Label", -123.45, 345).Current;
			//Assert.AreEqual(2, iRef.SequenceNo);

			//RecordCode paRef = csnw.WriteArrayRecord(new long[]{10, 20, 30, 40, 50}).Current;
		}
	}
}
