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
		private readonly CsnForSer forCore = null;
		private readonly CsnTimeZones Ctzs = null;
		private readonly JsonSer jcore = null;
		private readonly JsonObjSer jobjcore = new JsonObjSer();
		private StreamWriter sw = null;

		public PerfSer()
		{
			this.Ctzs = TimeZoneService.S.GetTimeZones();
			this.core = new CsnSer();
			this.forCore = new CsnForSer();
			this.jcore = new JsonSer(Performance.JsonSer.longNames);
		}

		[IterationSetup]
		public void IterationSetup()
		{
			this.sw = new StreamWriter(Stream.Null, Encoding.UTF8);
		}

		[Benchmark]
		public void CsnSer()
		{
			this.core.Serialize(this.Ctzs, this.sw);
		}

		[Benchmark]
		public void JsonSer()
		{
			this.jcore.Serialize(this.Ctzs, this.sw);
		}

		//[Benchmark]
		//public void JsonObjSer()
		//{
		//	this.jobjcore.Serialize(this.Ctzs, this.sw);
		//}

		//[Benchmark]
		//public void JsonDeser()
		//{
		//	this.jcore.Deserialize(new StreamReader(Resources.ResourceManager.GetStream("tz_json_long")));
		//}

		//[Benchmark]
		//public void CsnForSer()
		//{
		//	this.forCore.Serialize(this.Ctzs, this.sw);
		//}
	}
}
