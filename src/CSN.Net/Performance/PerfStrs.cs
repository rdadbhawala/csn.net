﻿using Abstraction.Csn;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Performance
{
    public class PerfStrs
    {
		private StreamWriter sw = null;
		private Config cfg = null;
		private readonly long[] valueArr = new long[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		//private readonly string[] valueArr = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
		//private readonly DateTime[] valueArr = new DateTime[] { DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now };

		public PerfStrs()
		{
		}

		[IterationSetup]
		public void IterationSetup()
		{
			this.cfg = Config.CreateDefaultConfig();
			this.sw = new StreamWriter(Stream.Null, Encoding.UTF8);
		}

		[Benchmark]
		public void Csn()
		{
			Writer w = new Writer(this.sw, this.cfg);
			RecordCode rc = w.WriteTypeDefRecord("T", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9");
			for (long i = 0; i < 100000; i+=10)
			{
				IFieldWriter fw = w.WriteInstanceFields(rc);
				for (long j = 0; j < 10; j++)
				{
					fw.W(this.valueArr[j]);
				}
			}
			this.sw.Flush();
		}

		[Benchmark]
		public void Json()
		{
			JsonTextWriter jtw = new JsonTextWriter(this.sw);
			jtw.WriteStartArray();
			for (long i = 0; i < 100000; i += 10)
			{
				jtw.WriteStartObject();
				for (long j = 0; j < 10; j++)
				{
					jtw.WritePropertyName(j.ToString());
					jtw.WriteValue(this.valueArr[j]);
				}
				jtw.WriteEndObject();
			}
			jtw.WriteEndArray();
		}
	}
}
