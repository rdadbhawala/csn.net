using Abstraction.Csn;
using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Performance
{
	public class PerfInvocations
	{
		private readonly string[] names = { "ABCDE", "FGHIJ", "LKMNO", "PQRST", "UVWXY", "Z" };
		private StreamWriter sw = null;
		private Writer w = null;
		private readonly int limit = 100000;

		[IterationSetup]
		public void IterationSetup()
		{
			sw = new StreamWriter(new MemoryStream(), Encoding.UTF8);
			w = new Writer(sw);
		}

		[Benchmark]
		public void Types()
		{
			w.WriteTypeDefRecord("TY", names[0], names[1], names[2], names[3], names[4], names[5]);
			for (int ctr = 0; ctr < this.limit; ctr++)
			{
				w.WriteTypeDefRecord("TY", names[0], names[1], names[2], names[3], names[4], names[5]);
			}
		}

		[Benchmark]
		public void Instances()
		{
			RecordCode rc = w.WriteTypeDefRecord("TY", names[0], names[1], names[2], names[3], names[4], names[5]).Current;
			for (int ctr = 0; ctr < this.limit; ctr++)
			{
				w.WriteInstanceRecord(rc, names[0], names[1], names[2], names[3], names[4], names[5]);
			}
		}

		[Benchmark]
		public void Fields()
		{
			RecordCode rc = w.WriteTypeDefRecord("TY", names[0], names[1], names[2], names[3], names[4], names[5]).Current;
			for (int ctr = 0; ctr < this.limit; ctr++)
			{
				w.WriteInstanceFields(rc).W(names[0]).W(names[1]).W(names[2]).W(names[3]).W(names[4]).W(names[5]);
			}
		}

		[Benchmark]
		public void Arrays()
		{
			RecordCode rc = w.WriteTypeDefRecord("TY", names[0], names[1], names[2], names[3], names[4], names[5]).Current;
			for (int ctr = 0; ctr < this.limit; ctr++)
			{
				w.WriteArrayRecord(names);
			}
		}
	}
}
