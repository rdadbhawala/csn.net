﻿using System;
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

			RecordCode tRef = csnw.WriteTypeDefRecord("AllPrimitives", "BooleanTrue", "BooleanFalse", "DateTime", "String", "Real", "Integer");
			Assert.AreEqual(1, tRef.SequenceNo, "Ref Type - All Primitives");

			RecordCode iRef = csnw.WriteInstanceRecord(tRef, true, false, new DateTime(2019, 03, 12, 19, 24, 33, 567, DateTimeKind.Utc), "Label", -123.45, 345);
			Assert.AreEqual(2, iRef.SequenceNo);

			RecordCode paRef = csnw.WriteArrayRecord(new int[]{10, 20, 30, 40, 50});

			// read the contents
			sWriter.Flush();
			StreamReader sReader = new StreamReader(mstream);
			mstream.Position = 0;
			String str = sReader.ReadToEnd();

			Assert.AreEqual(TestResources.AllPrimitives, str);
		}
	}
}
