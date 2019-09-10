using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace Performance
{
	class JsonObjSer
		: ISerializer
	{
		public CsnTimeZones Deserialize(StreamReader sw)
		{
			throw new NotImplementedException();
		}

		public void Serialize(CsnTimeZones ctzs, StreamWriter sw)
		{
			JsonSerializer json = new JsonSerializer();
			json.Serialize(sw, ctzs);
		}
	}
}
