using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Performance
{
	public class PerfSer
	{
		private readonly CsnSer core = null;
		private readonly CsnTimeZones Ctzs = null;
		private readonly JsonSer jcore = null;
		private readonly JsonObjSer jobjcore = new JsonObjSer();
		private StreamWriter sw = null;

		public PerfSer()
		{
			this.Ctzs = TimeZoneService.S.GetTimeZones();
			this.core = new CsnSer();
			this.jcore = new JsonSer(Performance.JsonSer.longNames);
		}

		[IterationSetup]
		public void IterationSetup()
		{
			this.sw = new StreamWriter(Stream.Null, Encoding.UTF8);
		}

		[Benchmark]
		public void JsonSer()
		{
			this.jcore.Serialize(this.Ctzs, this.sw);
		}

		[Benchmark]
		public void JsonObjSer()
		{
			this.jobjcore.Serialize(this.Ctzs, this.sw);
		}

		[Benchmark]
		public void CsnSer()
		{
			this.core.Serialize(this.Ctzs, this.sw);
		}
	}
}
