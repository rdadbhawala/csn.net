using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Performance
{
    interface ISerializer
    {
		void Serialize(CsnTimeZones ctzs, StreamWriter sw);
    }
}
