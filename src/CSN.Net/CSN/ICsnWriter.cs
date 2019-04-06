// <copyright file="ICsnWriter.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
using System;

	/// <summary>
	/// ICsnWriter is the raw CSN Writer.
	/// </summary>
	public interface ICsnWriter
    {
		/// <summary>
		/// WriteFieldString writes a field with string value.
		/// </summary>
		/// <param name="pValue">String value to write.</param>
		void WriteFieldString(string pValue);

		/// <summary>
		/// WriteFieldLong writes a field with a long value.
		/// </summary>
		/// <param name="pValue">Long value to write.</param>
		void WriteFieldLong(long pValue);

		/// <summary>
		/// WriteFieldDateTime writes a field with a DateTime value.
		/// </summary>
		/// <param name="pValue">DateTime value to write.</param>
		void WriteFieldDateTime(DateTime pValue);
    }
}
