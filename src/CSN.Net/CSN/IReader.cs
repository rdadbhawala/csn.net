using System;
using System.Collections.Generic;
using System.Text;

namespace Abstraction.Csn
{
    public interface IReader
    {
		void Read(IRead callback);
    }

	public interface IRead
	{
		void Read(VersionRecord verRec);
		void Read(TypeDefRecord typeRec);
		void Read(InstanceRecord instRec);
		void Read(Record rec, int index, bool value);
		void Read(Record rec, int index, long value);
	}
}
