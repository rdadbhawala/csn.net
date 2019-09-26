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
	public static class PerfBool
	{
		public static int ctr = 10000;
		public static bool value = true;

		public static void Program()
		{
			//BenchmarkRunner.Run<PerfBoolCsnSer>();
			//BenchmarkRunner.Run<PerfBoolJsonSer>();
			BenchmarkRunner.Run<PerfBoolCsnDeser>();
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
			public void PerfBoolJsonDeserDo()
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
				IWriterField fw = w.WriteArray();
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

				public override void ReadValue(ValueRecord rec, int index, bool value)
				{
				}
			}
		}
	}
}
