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
	public static class PerfString
	{
		public static long ctr = 100000;
		public const string value = "ABCDEFGHIJ"; // "\"AB\\CD\\E\""; // 

		public static void Program()
		{
			//BenchmarkRunner.Run<PerfStringCsnSer>();
			//BenchmarkRunner.Run<PerfStringJsonSer>();
			BenchmarkRunner.Run<PerfStringCsnDeser>();
			//BenchmarkRunner.Run<PerfStringJsonDeser>();
		}

		private static Stream GetStream()
		{
			return Stream.Null;
		}

		public class PerfStringJsonSer
		{

			StreamWriter sw = null;
			JsonTextWriter jw = null;
			internal Stream tgt = PerfString.GetStream();

			[IterationSetup]
			public void Setup()
			{
				sw = new StreamWriter(tgt, Encoding.UTF8);
				jw = new JsonTextWriter(sw);
			}

			[Benchmark]
			public void PerfStringJsonSerDo()
			{
				jw.WriteStartArray();
				for (long i = 0; i < PerfString.ctr; i++)
				{
					jw.WriteValue(PerfString.value);
				}
				jw.WriteEndArray();
				jw.Flush();
			}
		}

		public class PerfStringJsonDeser
		{
			Stream mStream = new MemoryStream();
			StreamReader sr = null;

			public PerfStringJsonDeser()
			{
				PerfStringJsonSer ser = new PerfStringJsonSer()
				{
					tgt = mStream
				};
				ser.Setup();
				ser.PerfStringJsonSerDo();
			}

			[IterationSetup]
			public void Setup()
			{
				mStream.Position = 0;
				sr = new StreamReader(mStream);
			}

			[Benchmark]
			public void PerfStringJsonDeserDo()
			{
				JsonTextReader jReader = new JsonTextReader(sr);
				while (jReader.Read())
				{ }
			}
		}

		public class PerfStringCsnSer
		{
			StreamWriter sw = null;
			Writer w = null;
			internal Stream tgt = PerfString.GetStream();

			[IterationSetup]
			public void Setup()
			{
				sw = new StreamWriter(tgt, Encoding.UTF8);
				w = new Writer(sw);
			}

			[Benchmark]
			public void PerfStringCsnSerDo()
			{
				IWriterField fw = w.WriteArray();
				for (long i = 0; i < PerfString.ctr; i++)
				{
					fw.W(PerfString.value);
				}
				sw.Flush();
			}
		}

		public class PerfStringCsnDeser
		{
			Stream mStream = new MemoryStream();
			StreamReader sr = null;

			public PerfStringCsnDeser()
			{
				PerfStringCsnSer ser = new PerfStringCsnSer
				{
					tgt = mStream
				};
				ser.Setup();
				ser.PerfStringCsnSerDo();
			}

			[IterationSetup]
			public void Setup()
			{
				mStream.Position = 0;
				sr = new StreamReader(new BufferedStream(mStream), Encoding.UTF8);
			}

			[Benchmark]
			public void PerfStringCsnDeserDo()
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

				public override void ReadValue(ValueRecord rec, int index, string value)
				{
				}
			}
		}
	}
}
