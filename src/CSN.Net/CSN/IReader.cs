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
		void Read(VersionRecord vr);
		void Read(TypeDefRecord rec);
	}
}
