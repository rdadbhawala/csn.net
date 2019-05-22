// <copyright file="FieldDateTime.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System;
	using System.Globalization;
	using System.IO;

	/// <summary>
	/// DateTime field handling.
	/// </summary>
	internal class FieldDateTime
		: FieldBase<DateTime>
    {
		/// <summary>
		/// Singleton instance.
		/// </summary>
		public static readonly FieldDateTime F = new FieldDateTime();

		private readonly string formatString = "yyyyMMddTHHmmssfffffffK";

		private readonly char[] sArrDt = { 'D', '0', '0', '0', '0', '0', '0', '0', '0', 'T', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0' };

		private const char cZero = '0';
		private const char isoT = 'T';
		private const char isoZ = 'Z';
		private const long tickFactor = 10000000L;
		private const char dateSep = '-';
		private const char timeSep = ':';
		private const char secondSep = '.';

		private char[] chDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

		private FieldDateTime()
			: base(Constants.ArrayCode.DateTime)
		{
			// nothing
		}

		/// <summary>
		/// Write a DateTime field.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		/// <param name="field">value to write.</param>
		public override void WriteField(StreamWriter sw, DateTime field)
		{
			sw.Write(Constants.DefaultFieldSeparator);
			sw.Write(Constants.DateTimePrefix);

			//sw.Write(field.ToString(this.formatString));

			//sw.Write(field.Year);
			//sw.Write('-');
			//sw.Write(field.Month);
			//sw.Write('-');
			//sw.Write(field.Day);
			//sw.Write('T');
			//sw.Write(field.Hour);
			//sw.Write(':');
			//sw.Write(field.Minute);
			//sw.Write(':');
			//sw.Write(field.Second);
			//sw.Write('.');

			//WriteYear(sw, field.Year);
			//Write2DigitInt(sw, field.Month);
			//Write2DigitInt(sw, field.Day);
			//sw.Write(isoT);
			//Write2DigitInt(sw, field.Hour);
			//Write2DigitInt(sw, field.Minute);
			//Write2DigitInt(sw, field.Second);
			//sw.Write(field.Ticks % tickFactor);
			//switch (field.Kind)
			//{
			//	case DateTimeKind.Utc: sw.Write(isoZ); break;
			//	case DateTimeKind.Local:
			//		TimeSpan ts = TimeZoneInfo.Local.GetUtcOffset(field);
			//		sw.Write(ts.Ticks > 0 ? '+' : '-');
			//		Write2DigitInt(sw, ts.Hours);
			//		Write2DigitInt(sw, ts.Minutes);
			//		break;
			//}

			char[] arrDt = { 'D', '0', '0', '0', '0', '0', '0', '0', '0', 'T', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0' };
			WriteIntToCharArr(arrDt, 1, 4, field.Year);
			WriteIntToCharArr(arrDt, 5, 2, field.Month);
			WriteIntToCharArr(arrDt, 7, 2, field.Day);
			WriteIntToCharArr(arrDt, 10, 2, field.Hour);
			WriteIntToCharArr(arrDt, 12, 2, field.Minute);
			WriteIntToCharArr(arrDt, 14, 2, field.Second);
			WriteIntToCharArr(arrDt, 16, 7, (int)(field.Ticks % tickFactor));
			sw.Write(arrDt, 0, 23);

			//if (field.Kind == DateTimeKind.Utc)
			//{
			//	sw.Write(isoZ);
			//}
			//else if (field.Kind == DateTimeKind.Local)
			//{
			//	TimeSpan ts = TimeZoneInfo.Local.GetUtcOffset(field);
			//	char[] arrTz = { '+', '0', '0', '0', '0' };
			//	arrTz[0] = (ts.Ticks > 0 ? '+' : '-');
			//	WriteIntToCharArr(arrTz, 1, 2, ts.Hours);
			//	WriteIntToCharArr(arrTz, 3, 2, ts.Minutes);
			//	sw.Write(arrTz);
			//}

			switch (field.Kind)
			{
				case DateTimeKind.Utc: sw.Write(isoZ); break;
				case DateTimeKind.Local:
					TimeSpan ts = TimeZoneInfo.Local.GetUtcOffset(field);
					char[] arrTz = { (ts.Ticks > 0 ? '+' : '-'), '0', '0', '0', '0' };
					WriteIntToCharArr(arrTz, 1, 2, ts.Hours);
					WriteIntToCharArr(arrTz, 3, 2, ts.Minutes);
					sw.Write(arrTz, 0, 5);
					break;
			}
		}

		private void WriteIntToCharArr(char[] arrDt, int index, int len, int value)
		{
			for (int i = index + len - 1; i >= index && value > 0; i--)
			{
				arrDt[i] = chDigits[value % 10];
				value /= 10;
			}
		}

		private void Write2DigitInt(StreamWriter sw, int value)
		{
			char[] ch = new char[2];
			ch[1] = chDigits[value % 10];
			value /= 10;
			ch[0] = chDigits[value];

			sw.Write(ch);
			//if (value < 10)
			//{
			//	sw.Write(cZero);
			//}
			//sw.Write(value);
		}

		private void WriteYear(StreamWriter sw, int year)
		{
			char[] chYear = { '0', '0', '0', chDigits[year % 10] };
			year /= 10;
			chYear[2] = chDigits[year % 10];
			year /= 10;
			chYear[1] = chDigits[year % 10];
			year /= 10;
			chYear[0] = chDigits[year % 10];

			//if (year >= 1000)
			//{
			//	// nothing
			//}
			//else if (year >= 100)
			//{
			//	sw.Write(cZero);
			//}
			//else if (year >= 10)
			//{
			//	sw.Write(cZero);
			//	sw.Write(cZero);
			//}
			//else if (year >= 0)
			//{
			//	sw.Write(cZero);
			//	sw.Write(cZero);
			//	sw.Write(cZero);
			//}

			//sw.Write(year.ToString());
		}
	}
}
