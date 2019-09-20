using System;
using System.Collections.Generic;
using System.Text;

namespace Csn
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

	public abstract class ValueRecord : Record
	{
		public ValueRecord(RecordCode rc) : base(rc)
		{
			this.Values = new List<object>();
		}

		public List<object> Values { get; private set; }
	}

	public class InstanceRecord : ValueRecord
	{
		public InstanceRecord(RecordCode rc, TypeDefRecord refRec) : base(rc)
		{
			this.Ref = refRec;
		}

		public TypeDefRecord Ref { get; internal set; }
		public object Tag { get; set; }
	}

	public class ArrayRecord : ValueRecord
	{
		public ArrayRecord(RecordCode rc) : base(rc)
		{ }
	}

	public class ArrayPrimitivesRecord : ArrayRecord
	{
		public ArrayPrimitivesRecord(RecordCode rc, PrimitiveType pType)
			: base(rc)
		{
			this.Primitive = pType;
		}

		public PrimitiveType Primitive { get; private set; }
	}

	public class ArrayRefsRecord : ValueRecord
	{
		public ArrayRefsRecord(RecordCode rc, TypeDefRecord refRec) : base(rc)
		{
			this.TypeRef = refRec;
		}

		public TypeDefRecord TypeRef { get; private set; }
	}
}
