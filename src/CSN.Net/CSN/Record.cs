using System;
using System.Collections.Generic;
using System.Text;

namespace Abstraction.Csn
{
    public abstract class Record
    {
		public RecordCode Code { get; }
    }

	public class VersionRecord : Record
	{
		public String Version { get; }
	}

	public class TypeDefRecord : Record
	{
		public string Name { get; internal set; }
		public string[] Members { get; internal set; }
	}

	public class InstanceRecord : Record
	{
		public RecordCode TypeReference { get; internal set; }
		public object[] Values { get; set; }
	}
}
