using Csn;
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
			//StreamWriter sw = new StreamWriter(mstream);
			//sw.Write(TestResources.AllPrimitives);
			//sw.Flush();
			//mstream.Position = 0;
			StreamReader sr = new StreamReader("d:\\Temp\\tz-csn.txt");

			IReader rdr = Reader.Singleton;
			rdr.Read(sr, new ReadCallback());
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
				Console.WriteLine("Instance " + instRec.SequenceNo + " of " + instRec.Ref.Name);
			}

			public void Read(ArrayRecord arrRec)
			{
				Console.WriteLine("Array " + arrRec.SequenceNo);
			}

			public void ReadValueNull(ValueRecord rec, int index)
			{
				Console.WriteLine(index + ") Value null");
			}

			public void ReadValue(ValueRecord rec, int index, bool value)
			{
				Console.WriteLine(index + ") Value " + value);
			}

			public void ReadValue(ValueRecord rec, int index, long value)
			{
				Console.WriteLine(index + ") Value " + value);
				//Console.ReadLine();
			}

			public void ReadValue(ValueRecord rec, int index, double value)
			{
				Console.WriteLine(index + ") Value " + value);
			}

			public void ReadValue(ValueRecord rec, int index, string value)
			{
				Console.WriteLine(index + ") Value " + value);
			}

			public void ReadValue(ValueRecord rec, int index, Record obj)
			{
				Console.WriteLine(index + ") Value #" + obj.SequenceNo);
			}

			public void ReadValue(ValueRecord rec, int index, DateTime value)
			{
				Console.WriteLine(index + ") Value " + value.ToString());
			}
		}

	}
}
