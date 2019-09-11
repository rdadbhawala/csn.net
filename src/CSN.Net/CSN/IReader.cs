using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Abstraction.Csn
{
    public interface IReader
    {
		void Read(StreamReader sReader, IRead callback);
    }

	public interface IRead
	{
		void Read(VersionRecord verRec);
		void Read(TypeDefRecord typeRec);
		void Read(InstanceRecord instRec);
		void Read(ArrayRefsRecord arrayRec);
		void Read(ArrayPrimitivesRecod arrRec);

		IReadValue GetReadValue();
	}

	public interface IReadValue
	{
		void ReadValueNull(ValueRecord rec, int index);
		//void ReadValue(ValueRecord rec, int index, PrimitiveNull value);
		void ReadValue(ValueRecord rec, int index, bool value);
		void ReadValue(ValueRecord rec, int index, long value);
		void ReadValue(ValueRecord rec, int index, double value);
		void ReadValue(ValueRecord rec, int index, string value);
		void ReadValue(ValueRecord rec, int index, Record value);
		void ReadValue(ValueRecord rec, int index, DateTime value);
	}
}
