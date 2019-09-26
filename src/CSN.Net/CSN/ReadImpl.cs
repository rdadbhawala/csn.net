using System;
using System.Collections.Generic;
using System.Text;

namespace Csn
{
	public class ReadImplExc : IRead, IReadValue
	{
		public virtual IReadValue GetReadValue()
		{
			throw new InvalidOperationException();
		}

		public virtual void Read(VersionRecord verRec)
		{
			throw new InvalidOperationException();
		}

		public virtual void Read(TypeDefRecord typeRec)
		{
			throw new InvalidOperationException();
		}

		public virtual void Read(InstanceRecord instRec)
		{
			throw new InvalidOperationException();
		}

		public virtual void Read(ArrayRecord arrRec)
		{
			throw new InvalidOperationException();
		}

		public virtual void ReadValue(ValueRecord rec, int index, bool value)
		{
			throw new InvalidOperationException();
		}

		public virtual void ReadValue(ValueRecord rec, int index, long value)
		{
			throw new InvalidOperationException();
		}

		public virtual void ReadValue(ValueRecord rec, int index, double value)
		{
			throw new InvalidOperationException();
		}

		public virtual void ReadValue(ValueRecord rec, int index, string value)
		{
			throw new InvalidOperationException();
		}

		public virtual void ReadValue(ValueRecord rec, int index, Record value)
		{
			throw new InvalidOperationException();
		}

		public virtual void ReadValue(ValueRecord rec, int index, DateTime value)
		{
			throw new InvalidOperationException();
		}

		public virtual void ReadValueNull(ValueRecord rec, int index)
		{
			throw new InvalidOperationException();
		}
	}

	public class ReadImplBlanks : IRead, IReadValue
	{
		public virtual IReadValue GetReadValue()
		{
			return this;
		}

		public virtual void Read(VersionRecord verRec)
		{ }

		public virtual void Read(TypeDefRecord typeRec)
		{ }

		public virtual void Read(InstanceRecord instRec)
		{ }

		public virtual void Read(ArrayRecord arrRec)
		{ }

		public virtual void ReadValue(ValueRecord rec, int index, bool value)
		{ }

		public virtual void ReadValue(ValueRecord rec, int index, long value)
		{ }

		public virtual void ReadValue(ValueRecord rec, int index, double value)
		{ }

		public virtual void ReadValue(ValueRecord rec, int index, string value)
		{ }

		public virtual void ReadValue(ValueRecord rec, int index, Record value)
		{ }

		public virtual void ReadValue(ValueRecord rec, int index, DateTime value)
		{ }

		public virtual void ReadValueNull(ValueRecord rec, int index)
		{ }
	}
}
