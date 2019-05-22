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

			char[] arrDt = { 'D', '0', '0', '0', '0', '0', '0', '0', '0', 'T', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0' };
			WriteIntToCharArr(arrDt, 1, 4, field.Year);
			WriteIntToCharArr(arrDt, 5, 2, field.Month);
			WriteIntToCharArr(arrDt, 7, 2, field.Day);
			WriteIntToCharArr(arrDt, 10, 2, field.Hour);
			WriteIntToCharArr(arrDt, 12, 2, field.Minute);
			WriteIntToCharArr(arrDt, 14, 2, field.Second);
			WriteIntToCharArr(arrDt, 16, 7, (int)(field.Ticks % tickFactor));
			sw.Write(arrDt, 0, 23);

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
	}
}
