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

			Config cfg = Config.CreateDefaultConfig();
			Writer csnw = new Writer(sWriter, cfg);

			int tRef = csnw.WriteTypeDefRecord("AllPrimitives", "BooleanTrue", "BooleanFalse", "DateTime", "String", "Real", "Integer");
			Assert.AreEqual(1, tRef, "Ref Type - All Primitives");

			int iRef = csnw.WriteInstanceRecord(tRef, true, false, new DateTime(2019,03,12,19,24,33,567, DateTimeKind.Utc), "Label", -123.45, 345);


			// read the contents
			sWriter.Flush();
			StreamReader sReader = new StreamReader(mstream);
			mstream.Position = 0;
			String str = sReader.ReadToEnd();

			Assert.AreEqual(TestResources.Basic, str);
		}
	}
}
