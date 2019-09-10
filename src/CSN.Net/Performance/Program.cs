using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.Text;

namespace Performance
{
    class Program
    {
        static void Main(string[] args)
        {
			//CsnTimeZones csnTzs = TimeZoneService.S.GetTimeZones();
			//CsnSer csn = new CsnSer();
			//WriteSer(csn, "d:\\Temp\\tz-csn.txt", csnTzs);
			//JsonSer json = new JsonSer(JsonSer.longNames);
			//WriteSer(json, "d:\\Temp\\tz-json-long.txt", csnTzs);
			//JsonSer json2 = new JsonSer(JsonSer.shortNames);
			//WriteSer(json2, "d:\\Temp\\tz-json-short.txt", csnTzs);

			//BenchmarkRunner.Run<PerfSer>();
			BenchmarkRunner.Run<PerfDeser>();

			Console.ReadLine();
		}

		static void WriteSer(ISerializer ser, String path, CsnTimeZones ctzs)
		{
			FileStream fs = new FileStream(path, FileMode.Create);
			StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
			ser.Serialize(ctzs, sw);
			sw.Flush();
			sw.Close();
		}
    }
}
