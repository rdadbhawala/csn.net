using Csn;
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
	public static class PerfLong
	{
		public static long ctr = 10000;

		public static void Program()
		{
			//BenchmarkRunner.Run<PerfLongCsnSer>();
			//BenchmarkRunner.Run<PerfLongJsonSer>();
			BenchmarkRunner.Run<PerfLongCsnDeser>();
			BenchmarkRunner.Run<PerfLongJsonDeser>();
		}

		private static Stream GetStream()
		{
			return Stream.Null;
		}

		public class PerfLongJsonSer
		{

			StreamWriter sw = null;
			JsonTextWriter jw = null;
			internal Stream tgt = PerfLong.GetStream();

			[IterationSetup]
			public void Setup()
			{
				sw = new StreamWriter(tgt, Encoding.UTF8);
				jw = new JsonTextWriter(sw);
			}

			[Benchmark]
			public void PerfLongJsonSerDo()
			{
				jw.WriteStartArray();
				for (long i = 0; i < PerfLong.ctr; i++)
				{
					jw.WriteValue(i);
				}
				jw.WriteEndArray();
				jw.Flush();
			}
		}

		public class PerfLongJsonDeser
		{
			Stream mStream = new MemoryStream();
			StreamReader sr = null;

			public PerfLongJsonDeser()
			{
				PerfLongJsonSer ser = new PerfLongJsonSer()
				{
					tgt = mStream
				};
				ser.Setup();
				ser.PerfLongJsonSerDo();
			}

			[IterationSetup]
			public void Setup()
			{
				mStream.Position = 0;
				sr = new StreamReader(mStream);
			}

			[Benchmark]
			public void PerfLongJsonDeserDo()
			{
				JsonTextReader jReader = new JsonTextReader(sr);
				while (jReader.Read())
				{ }
			}
		}

		public class PerfLongCsnSer
		{
			StreamWriter sw = null;
			Writer w = null;
			internal Stream tgt = PerfLong.GetStream();

			[IterationSetup]
			public void Setup()
			{
				sw = new StreamWriter(tgt, Encoding.UTF8);
				w = new Writer(sw);
			}

			[Benchmark]
			public void PerfLongCsnSerDo()
			{
				IWriterField fw = w.WriteArray();
				for (long i = 0; i < PerfLong.ctr; i++)
				{
					fw.W(i);
				}
			}
		}

		public class PerfLongCsnDeser
		{
			Stream mStream = new MemoryStream();
			StreamReader sr = null;

			public PerfLongCsnDeser()
			{
				PerfLongCsnSer ser = new PerfLongCsnSer
				{
					tgt = mStream
				};
				ser.Setup();
				ser.PerfLongCsnSerDo();
			}

			[IterationSetup]
			public void Setup()
			{
				mStream.Position = 0;
				sr = new StreamReader(mStream, Encoding.UTF8);
			}

			[Benchmark]
			public void PerfLongCsnDeserDo()
			{
				Reader.Singleton.Read(sr, new ReadRec());
			}

			class ReadRec : ReadImplExc, IRead, IReadValue
			{
				public override IReadValue GetReadValue()
				{
					return this;
				}

				public override void Read(VersionRecord verRec)
				{
				}

				public override void Read(ArrayRecord arrRec)
				{
				}

				public override void ReadValue(ValueRecord rec, int index, long value)
				{
				}
			}
		}
	}
}
