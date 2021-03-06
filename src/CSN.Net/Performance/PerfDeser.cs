﻿using Csn;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Performance
{
    public class PerfDeser
    {
		private readonly JsonSer jser = null;
		private StreamReader readerJson = null;
		private StreamReader readerCsn = null;

		public PerfDeser()
		{
			jser = new JsonSer(Performance.JsonSer.longNames);
		}

		[IterationSetup]
		public void Setup()
		{
			readerJson = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(Resources.tz_json_long)), Encoding.UTF8);
			//readerJson = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(Resources.tz_json_short)), Encoding.UTF8);
			readerCsn = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(Resources.tz_csn)), Encoding.UTF8);
		}

		//[Benchmark]
		public void JsonDeser()
		{
			JsonTextReader jReader = new JsonTextReader(readerJson);
			while (jReader.Read())
			{ }
		}

		[Benchmark]
		public void CsnDeser()
		{
			//CsnSer deser = new CsnSer();
			Reader.Singleton.Read(readerCsn, new CsnReadTz());
		}

		class CsnReadTz : ReadImplBlanks, IRead, IReadValue
		{
		}
	}
}
