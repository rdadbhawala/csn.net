using System;
using System.Collections.Generic;
using System.Text;

namespace Abstraction.Csn
{
    public abstract class Record
    {
		protected Record (RecordCode rc)
		{
			this.Code = rc;
		}
		public RecordCode Code { get; private set; }
    }

	public class VersionRecord : Record
	{
		public VersionRecord(RecordCode rc, String ver) : base(rc)
		{
			this.Version = ver;
		}

		public String Version { get; internal set; }
	}

	public class TypeDefRecord : Record
	{
		public TypeDefRecord(RecordCode rc, String pName) : base(rc)
		{
			this.Name = pName;
		}

		public string Name { get; internal set; }
		public string[] Members { get; internal set; }
	}

	public class InstanceRecord : Record
	{
		public InstanceRecord(RecordCode rc, TypeDefRecord refRec) : base(rc)
		{
			this.Ref = refRec;
		}

		public TypeDefRecord Ref { get; internal set; }
		public object Tag { get; set; }
	}

	public class ArrayRecord : Record
	{
		public ArrayRecord(RecordCode rc) : base(rc)
		{ }
	}
}
