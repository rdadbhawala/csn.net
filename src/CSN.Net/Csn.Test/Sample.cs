using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Abstraction.Csn.Test
{
	[TestClass]
	public class Sample
	{
		[TestMethod]
		public void Test()
		{
			// externals
			MemoryStream mstream = new MemoryStream();
			StreamWriter sWriter = new StreamWriter(mstream);

			// setup the CSN Parser
			Config cfg = Config.CreateDefaultConfig();
			Writer csnw = new Writer(sWriter, cfg);

			// 
		}
	}
}
