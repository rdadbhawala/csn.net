using System;
using System.IO;
using System.Text;

namespace Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
			CsnTimeZones csnTzs = TimeZoneService.S.GetTimeZones();
			Console.WriteLine(csnTzs.TimeZones.Length);

			CsnSer csn = new CsnSer();
			JsonSer json = new JsonSer();

			WriteSer(csn, "d:\\Temp\\tz-csn.txt", csnTzs);
			WriteSer(json, "d:\\Temp\\tz-json.txt", csnTzs);
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
