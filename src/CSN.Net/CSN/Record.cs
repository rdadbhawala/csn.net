using System;
using System.Collections.Generic;
using System.Text;

namespace Csn
{
    public abstract class Record
    {
		protected Record(long seqNo, RecordType recType)
		{
			this.mSequenceNo = seqNo;
			this.mRecType = recType;
		}

		private readonly long mSequenceNo;
		public long SequenceNo { get { return mSequenceNo; } }

		private readonly RecordType mRecType;
		public RecordType RecType { get { return mRecType; } }
    }

	public class VersionRecord : Record
	{
		public VersionRecord(String ver) : base(0, RecordType.Version)
		{
			this.Version = ver;
		}

		public String Version { get; internal set; }
	}

	public class TypeDefRecord : Record
	{
		public TypeDefRecord(long seqNo, String pName, string[] pMembers) : base(seqNo, RecordType.TypeDef)
		{
			this.Name = pName;
			this.Members = pMembers;
		}

		public string Name { get; internal set; }
		public string[] Members { get; internal set; }
	}

	public abstract class ValueRecord : Record
	{
		public ValueRecord(long seqNo, RecordType recType) : base(seqNo, recType)
		{
			//this.Values = new List<object>();
		}

		//public List<object> Values { get; private set; }
		public object[] Values { get; internal set; }
	}

	public class InstanceRecord : ValueRecord
	{
		public InstanceRecord(long seqNo, TypeDefRecord refRec) : base(seqNo, RecordType.Instance)
		{
			this.Ref = refRec;
		}

		public TypeDefRecord Ref { get; private set; }
		public object Tag { get; set; }
	}

	public class ArrayRecord : ValueRecord
	{
		public ArrayRecord(long seqNo) : base(seqNo, RecordType.Array)
		{ }
	}
}
