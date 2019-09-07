﻿using System;
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
			rdr.Read(new ReadCallback());


			//RecordCode tRef = csnw.WriteTypeDefRecord("AllPrimitives", "BooleanTrue", "BooleanFalse", "DateTime", "String", "Real", "Integer").Current;
			//Assert.AreEqual(1, tRef.SequenceNo, "Ref Type - All Primitives");

			//RecordCode iRef = csnw.WriteInstanceRecord(tRef, true, false, new DateTime(2019, 03, 12, 19, 24, 33, 567, DateTimeKind.Utc), "Label", -123.45, 345).Current;
			//Assert.AreEqual(2, iRef.SequenceNo);

			//RecordCode paRef = csnw.WriteArrayRecord(new long[]{10, 20, 30, 40, 50}).Current;
		}

		class ReadCallback : IRead, IReadValue
		{
			public IReadValue GetReadValue()
			{
				return this;
			}

			public void Read(VersionRecord verRec)
			{
				Console.WriteLine("Version" + verRec.Version);
			}

			public void Read(TypeDefRecord typeRec)
			{
				Console.WriteLine("Type Name: " + typeRec.Name);
			}

			public void Read(InstanceRecord instRec)
			{
				Console.WriteLine("Instance " + instRec.Code.SequenceNo + " of " + instRec.Ref.Name);
			}

			public void ReadValue(Record rec, int index, PrimitiveNull value)
			{
				Console.WriteLine(index + ") Value null");
			}

			public void ReadValue(Record rec, int index, bool value)
			{
				Console.WriteLine(index + ") Value " + value);
			}

			public void ReadValue(Record rec, int index, long value)
			{
				Console.WriteLine(index + ") Value " + value);
			}

			public void ReadValue(Record rec, int index, double value)
			{
				Console.WriteLine(index + ") Value " + value);
			}

			public void ReadValue(Record rec, int index, string value)
			{
				Console.WriteLine(index + ") Value " + value);
			}

			public void ReadValue(Record rec, int index, Record obj)
			{
				Console.WriteLine(index + ") Value " + obj.Code.SequenceNo);
			}
		}
	}
}
