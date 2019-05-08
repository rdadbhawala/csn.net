using System;

namespace FiguringItOut
{
	public class DateTimeFormat
	{
		static string format1 = "yyyyMMddTHHmmssfffffffK";
		public static void FormatO()
		{
			PrintDate(DateTimeKind.Unspecified, "O");
			PrintDate(DateTimeKind.Local, "O");
			PrintDate(DateTimeKind.Utc, "O");
			PrintDate(DateTimeKind.Unspecified, format1);
			PrintDate(DateTimeKind.Local, format1);
			PrintDate(DateTimeKind.Utc, format1);
		}

		private static void PrintDate(DateTimeKind kind, string f)
		{
			Console.WriteLine(getDateTime(kind).ToString(f, null));
			Console.WriteLine("{2}      {0} {1}", kind, f, getDateTime(kind).ToString(f));
			Console.ReadLine();
		}

		private static DateTime getDateTime (DateTimeKind kind)
		{
			return new DateTime(2009, 12, 21, 12, 34, 56, 789, kind);
		}
	}
}
