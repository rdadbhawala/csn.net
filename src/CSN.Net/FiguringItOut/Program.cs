using System;

namespace FiguringItOut
{
    class Program
    {
		static void Main(string[] args)
		{
			try
			{
				//Sample.PrintCsn();
				SampleReader.Read();
			}
			finally
			{
				Console.ReadLine();
			}
		}
    }
}
