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

			IReader rdr = Reader.Singleton;
			rdr.Read(sr, new ReadCallback());


			//RecordCode tRef = csnw.WriteTypeDefRecord("AllPrimitives", "BooleanTrue", "BooleanFalse", "DateTime", "String", "Real", "Integer").Current;
			//Assert.AreEqual(1, tRef.SequenceNo, "Ref Type - All Primitives");

			//RecordCode iRef = csnw.WriteInstanceRecord(tRef, true, false, new DateTime(2019, 03, 12, 19, 24, 33, 567, DateTimeKind.Utc), "Label", -123.45, 345).Current;
			//Assert.AreEqual(2, iRef.SequenceNo);

			//RecordCode paRef = csnw.WriteArrayRecord(new long[]{10, 20, 30, 40, 50}).Current;
		}

		class ReadCallback : ReadTest
		{
			public ReadCallback()
			{
				this.SetupVersionRecord("0.1.0");
				this.SetupTypeRecord(new RecordCode(RecordType.TypeDef, 1), "AllPrimitives", "BooleanTrue", "BooleanFalse", "DateTime", "String", "Real", "Integer");
				this.SetupInstanceOrArrayRefs(new RecordCode(RecordType.Instance, 2), 1).Setup(true).Setup(false).Setup(new DateTime(2019, 03, 12, 19, 24, 33, 567)).Setup("Label").Setup(-123.45).Setup(345);
				this.SetupArrayPrims(new RecordCode(RecordType.Array, 3), PrimitiveType.Int).Setup(10).Setup(20).Setup(30).Setup(40).Setup(50);
				this.SetupInstanceOrArrayRefs(new RecordCode(RecordType.Array, 4), 1).SetupRefs(2);
			}
		}
	}
}
