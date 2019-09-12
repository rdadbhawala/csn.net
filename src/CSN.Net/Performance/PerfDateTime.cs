using Abstraction.Csn;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Performance
{
	public static class PerfDateTime
	{
		public static int ctr = 10000;
		public static DateTime value = new DateTime(2019, 09, 12, 12, 28, 45);

		public static void Program()
		{
			//long val1 = 44, val2 = 45, val3 = 46;
			//Console.WriteLine(-val1 / 10);
			//Console.WriteLine(-val2 / 10);
			//Console.WriteLine(-val3 / 10);
			BenchmarkRunner.Run<PerfDateTimeCsnSer>();
			BenchmarkRunner.Run<PerfDateTimeJsonSer>();
			//BenchmarkRunner.Run<PerfDateTimeCsnDeser>();
			//BenchmarkRunner.Run<PerfDateTimeJsonDeser>();
		}

		private static Stream GetStream()
		{
			return Stream.Null;
		}

		public class PerfDateTimeJsonSer
		{

			StreamWriter sw = null;
			JsonTextWriter jw = null;
			internal Stream tgt = PerfDateTime.GetStream();

			[IterationSetup]
			public void Setup()
			{
				sw = new StreamWriter(tgt, Encoding.UTF8);
				jw = new JsonTextWriter(sw);
			}

			[Benchmark]
			public void PerfDateTimeJsonSerDo()
			{
				jw.WriteStartArray();
				for (int i = 0; i < PerfDateTime.ctr; i++)
				{
					jw.WriteValue(PerfDateTime.value);
				}
				jw.WriteEndArray();
				jw.Flush();
			}
		}

		public class PerfDateTimeJsonDeser
		{
			Stream mStream = new MemoryStream();
			StreamReader sr = null;

			public PerfDateTimeJsonDeser()
			{
				PerfDateTimeJsonSer ser = new PerfDateTimeJsonSer()
				{
					tgt = mStream
				};
				ser.Setup();
				ser.PerfDateTimeJsonSerDo();
			}

			[IterationSetup]
			public void Setup()
			{
				mStream.Position = 0;
				sr = new StreamReader(mStream);
			}

			[Benchmark]
			public void PerfBookJsonDeserDo()
			{
				JsonTextReader jReader = new JsonTextReader(sr);
				while (jReader.Read())
				{ }
			}
		}

		public class PerfDateTimeCsnSer
		{
			StreamWriter sw = null;
			Writer w = null;
			internal Stream tgt = PerfDateTime.GetStream();

			[IterationSetup]
			public void Setup()
			{
				sw = new StreamWriter(tgt, Encoding.UTF8);
				w = new Writer(sw);
			}

			[Benchmark]
			public void PerfDateTimeCsnSerDo()
			{
				IWriterField fw = w.WriteArray(PrimitiveType.DateTime);
				for (int i = 0; i < PerfDateTime.ctr; i++)
				{
					fw.W(PerfDateTime.value);
				}
			}
		}

		public class PerfDateTimeCsnDeser
		{
			Stream mStream = new MemoryStream();
			StreamReader sr = null;

			public PerfDateTimeCsnDeser()
			{
				PerfDateTimeCsnSer ser = new PerfDateTimeCsnSer
				{
					tgt = mStream
				};
				ser.Setup();
				ser.PerfDateTimeCsnSerDo();
			}

			[IterationSetup]
			public void Setup()
			{
				mStream.Position = 0;
				sr = new StreamReader(mStream, Encoding.UTF8);
			}

			[Benchmark]
			public void PerfDateTimeCsnDeserDo()
			{
				Reader.Singleton.Read(sr, new ReadRec());
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

				public void Read(ArrayPrimitivesRecod arrRec)
				{
				}

				public void ReadValueNull(ValueRecord rec, int index)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, bool value)
				{
				}

				public void ReadValue(ValueRecord rec, int index, long value)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, double value)
				{
					throw new InvalidOperationException();
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
