// <copyright file="FieldDateTime.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System;
	using System.Globalization;
	using System.IO;

	/// <summary>
	/// DateTime field handling
	/// </summary>
    internal class FieldDateTime
    {
		/// <summary>
		/// Singleton instance
		/// </summary>
		public static readonly FieldDateTime F = new FieldDateTime();

		private readonly string formatString = "yyyyMMddTHHmmssfffffffK";

		private FieldDateTime()
		{
			// nothing
		}

		/// <summary>
		/// Write a DateTime field
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		/// <param name="field">value to write</param>
		public void WriteField(StreamWriter sw, DateTime field)
		{
			sw.Write(Constants.DefaultFieldSeparator);
			sw.Write(Constants.DateTimePrefix);
			sw.Write(field.ToString(this.formatString));
		}
    }
}
