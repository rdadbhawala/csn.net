﻿using Abstraction.Csn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FiguringItOut
{
	class SampleReader
	{
		public static void Read()
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
