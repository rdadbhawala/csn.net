using Abstraction.Csn;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Performance
{
	public static class PerfDouble
	{
		public static long ctr = 10000;
		public static double[] values = null;

		static PerfDouble()
		{
			values = new double[ctr];
			for (int i = 0; i < ctr; i++)
			{
				values[i] = i / 100.0;
			}
		}

		public static void Program()
		{
			//Console.WriteLine(Math.Floor(123.0));
			//Console.WriteLine(Math.Floor(123.123));
			//Console.WriteLine(Math.Floor(123.678));
			//Console.WriteLine(Math.Ceiling(-123.0));
			//Console.WriteLine(Math.Ceiling(-123.123));
			//Console.WriteLine(Math.Ceiling(-123.678));
			//Console.WriteLine((-123.123) - Math.Ceiling(-123.123));
			//Console.WriteLine((123.123) - Math.Floor(123.123));
			//Console.WriteLine((123.456).ToString("#.0"));

			//BenchmarkRunner.Run<PerfDoubleCsnSer>();
			//BenchmarkRunner.Run<PerfDoubleJsonSer>();
			BenchmarkRunner.Run<PerfDoubleCsnDeser>();
			BenchmarkRunner.Run<PerfDoubleJsonDeser>();

			//PerfDoubleCsnSer ser = new PerfDoubleCsnSer();
			//ser.Setup();
			//ser.PerfDoubleCsnSerDo();
			//ser.tgt.Position = 0;
			//Console.WriteLine(new StreamReader(ser.tgt, Encoding.UTF8).ReadToEnd());
		}

		private static Stream GetStream()
		{
			return new MemoryStream();
		}

		public class PerfDoubleJsonSer
		{

			StreamWriter sw = null;
			JsonTextWriter jw = null;
			internal Stream tgt = PerfDouble.GetStream();

			[IterationSetup]
			public void Setup()
			{
				sw = new StreamWriter(tgt, Encoding.UTF8);
				jw = new JsonTextWriter(sw);
			}

			[Benchmark]
			public void PerfDoubleJsonSerDo()
			{
				jw.WriteStartArray();
				for (long i = 0; i < PerfDouble.ctr; i++)
				{
					jw.WriteValue(values[i]);
				}
				jw.WriteEndArray();
				jw.Flush();
			}
		}

		public class PerfDoubleJsonDeser
		{
			Stream mStream = new MemoryStream();
			StreamReader sr = null;

			public PerfDoubleJsonDeser()
			{
				PerfDoubleJsonSer ser = new PerfDoubleJsonSer()
				{
					tgt = mStream
				};
				ser.Setup();
				ser.PerfDoubleJsonSerDo();
			}

			[IterationSetup]
			public void Setup()
			{
				mStream.Position = 0;
				sr = new StreamReader(mStream);
			}

			[Benchmark]
			public void PerfDoubleJsonDeserDo()
			{
				JsonTextReader jReader = new JsonTextReader(sr);
				while (jReader.Read())
				{ }
			}
		}

		public class PerfDoubleCsnSer
		{
			StreamWriter sw = null;
			Writer w = null;
			internal Stream tgt = PerfDouble.GetStream();

			[IterationSetup]
			public void Setup()
			{
				sw = new StreamWriter(tgt, Encoding.UTF8);
				w = new Writer(sw);
			}

			[Benchmark]
			public void PerfDoubleCsnSerDo()
			{
				IWriterField fw = w.WriteArray(PrimitiveType.Int);
				for (long i = 0; i < PerfDouble.ctr; i++)
				{
					fw.W(values[i]);
				}
				sw.Flush();
			}
		}

		public class PerfDoubleCsnDeser
		{
			Stream mStream = new MemoryStream();
			StreamReader sr = null;
			ReadRec rr = null;

			public PerfDoubleCsnDeser()
			{
				PerfDoubleCsnSer ser = new PerfDoubleCsnSer
				{
					tgt = mStream
				};
				ser.Setup();
				ser.PerfDoubleCsnSerDo();
			}

			[IterationSetup]
			public void Setup()
			{
				mStream.Position = 0;
				sr = new StreamReader(mStream, Encoding.UTF8);
				rr = new ReadRec();
			}

			[Benchmark]
			public void PerfDoubleCsnDeserDo()
			{
				Reader.Singleton.Read(sr, rr);
			}

			class ReadRec : IRead, IReadValue
			{
				public IReadValue GetReadValue()
				{
					return this;
				}

				public void Read(VersionRecord verRec)
				{
				}

				public void Read(TypeDefRecord typeRec)
				{
					throw new InvalidOperationException();
				}

				public void Read(InstanceRecord instRec)
				{
					throw new InvalidOperationException();
				}

				public void Read(ArrayRefsRecord arrayRec)
				{
					throw new InvalidOperationException();
				}

				public void Read(ArrayPrimitivesRecord arrRec)
				{
				}

				public void ReadValueNull(ValueRecord rec, int index)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, bool value)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, long value)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, double value)
				{
				}

				public void ReadValue(ValueRecord rec, int index, string value)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, Record value)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, DateTime value)
				{
					throw new InvalidOperationException();
				}
			}
		}
	}
}
