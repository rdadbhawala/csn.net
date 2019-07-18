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

		IReadValue GetReadValue();
	}

	public interface IReadValue
	{
		void ReadValue(Record rec, int index, PrimitiveNull value);
		void ReadValue(Record rec, int index, bool value);
		void ReadValue(Record rec, int index, long value);
		void ReadValue(Record rec, int index, double value);
		void ReadValue(Record rec, int index, string value);
		void ReadValue(Record rec, int index, Record obj);
	}
}
