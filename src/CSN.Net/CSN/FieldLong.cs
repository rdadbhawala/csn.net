// <copyright file="FieldLong.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Long Field Writer
	/// </summary>
	internal class FieldLong
		: FieldBase<long>
    {
		/// <summary>
		/// Singleton FieldLong instance
		/// </summary>
		public static readonly FieldLong F = new FieldLong();

		private FieldLong()
		{
			// nothing
		}

		/// <summary>
		/// Write a Long value to a CSN Field
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		/// <param name="value">Value to write</param>
		public override void WriteField(StreamWriter sw, long value)
		{
			sw.Write(Constants.DefaultFieldSeparator);
			sw.Write(value);
		}
    }
}
