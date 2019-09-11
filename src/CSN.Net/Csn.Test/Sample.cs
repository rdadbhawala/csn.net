using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Abstraction.Csn.Test
{
	[TestClass]
	public class Sample
	{
		[TestMethod]
		public void Test()
		{
			// externals
			MemoryStream mstream = new MemoryStream();
			StreamWriter sWriter = new StreamWriter(mstream);

			// setup the CSN Parser

			IWriter csnw = new Writer(sWriter);

			RecordCode tRef = csnw.WriteTypeDefRecord("AllPrimitives", "BooleanTrue", "BooleanFalse", "DateTime", "String", "Real", "Integer").Current;
			Assert.AreEqual(1, tRef.SequenceNo, "Ref Type - All Primitives");

			RecordCode iRef = csnw.WriteInstanceRecord(tRef).W(true).W(false).W(new DateTime(2019, 03, 12, 19, 24, 33, 567, DateTimeKind.Utc)).W("Label").W(-123.45).W(345).Current;
			Assert.AreEqual(2, iRef.SequenceNo);

			RecordCode paRef = csnw.WriteArrayRecord(new long[]{10, 20, 30, 40, 50}).Current;

			RecordCode objRef = csnw.WriteArrayRecord(tRef, new RecordCode[] {iRef}).Current;

			// read the contents
			sWriter.Flush();
			StreamReader sReader = new StreamReader(mstream);
			mstream.Position = 0;
			String str = sReader.ReadToEnd();

			Assert.AreEqual(TestResources.AllPrimitives, str);
		}
	}
}
