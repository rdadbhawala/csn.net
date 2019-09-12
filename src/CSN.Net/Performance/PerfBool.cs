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
	public static class PerfBool
	{
		public static int ctr = 10000;
		public static bool value = true;

		public static void Program()
		{
			BenchmarkRunner.Run<PerfBoolCsnSer>();
			BenchmarkRunner.Run<PerfBoolJsonSer>();
			//BenchmarkRunner.Run<PerfBoolCsnDeser>();
			//BenchmarkRunner.Run<PerfBoolJsonDeser>();
		}

		private static Stream GetStream()
		{
			return new MemoryStream();
		}

		public class PerfBoolJsonSer
		{

			StreamWriter sw = null;
			JsonTextWriter jw = null;
			internal Stream tgt = PerfBool.GetStream();

			[IterationSetup]
			public void Setup()
			{
				sw = new StreamWriter(tgt, Encoding.UTF8);
				jw = new JsonTextWriter(sw);
			}

			[Benchmark]
			public void PerfBoolJsonSerDo()
			{
				jw.WriteStartArray();
				for (int i = 0; i < PerfBool.ctr; i++)
				{
					jw.WriteValue(PerfBool.value);
				}
				jw.WriteEndArray();
				jw.Flush();
			}
		}

		public class PerfBoolJsonDeser
		{
			Stream mStream = new MemoryStream();
			StreamReader sr = null;

			public PerfBoolJsonDeser()
			{
				PerfBoolJsonSer ser = new PerfBoolJsonSer()
				{
					tgt = mStream
				};
				ser.Setup();
				ser.PerfBoolJsonSerDo();
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

		public class PerfBoolCsnSer
		{
			StreamWriter sw = null;
			Writer w = null;
			internal Stream tgt = PerfBool.GetStream();

			[IterationSetup]
			public void Setup()
			{
				sw = new StreamWriter(tgt, Encoding.UTF8);
				w = new Writer(sw);
			}

			[Benchmark]
			public void PerfBoolCsnSerDo()
			{
				IWriterField fw = w.WriteArray(PrimitiveType.Bool);
				for (int i = 0; i < PerfBool.ctr; i++)
				{
					fw.W(PerfBool.value);
				}
			}
		}

		public class PerfBoolCsnDeser
		{
			Stream mStream = new MemoryStream();
			StreamReader sr = null;

			public PerfBoolCsnDeser()
			{
				PerfBoolCsnSer ser = new PerfBoolCsnSer
				{
					tgt = mStream
				};
				ser.Setup();
				ser.PerfBoolCsnSerDo();
			}

			[IterationSetup]
			public void Setup()
			{
				mStream.Position = 0;
				sr = new StreamReader(mStream, Encoding.UTF8);
			}

			[Benchmark]
			public void PerfBoolCsnDeserDo()
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
