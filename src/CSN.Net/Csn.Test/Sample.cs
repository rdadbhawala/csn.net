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

			// setup the CSN Parser
			Config cfg = Config.CreateDefaultConfig();
			Writer csnw = new Writer(mstream, cfg);

			// 
		}
	}
}
