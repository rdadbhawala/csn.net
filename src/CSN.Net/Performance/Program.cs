using System;

namespace Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
			CsnTimeZones csnTzs = TimeZoneService.S.GetTimeZones();
			Console.WriteLine(csnTzs.TimeZones.Length);

			ISerializer ser = new CsnSer();
			ser.Serialize(csnTzs, System.Console.OpenStandardOutput());

			//Console.ReadLine();
        }
    }
}
