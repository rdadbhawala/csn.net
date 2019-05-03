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
		/// Write the Version Record to the Stream.
		/// </summary>
		void WriteVersionRecord();

		/*
		///// <summary>
		///// WriteFieldString writes a field with string value.
		///// </summary>
		///// <param name="pValue">String value to write.</param>
		//void WriteFieldString(string pValue);

		///// <summary>
		///// WriteFieldLong writes a field with a long value.
		///// </summary>
		///// <param name="pValue">Long value to write.</param>
		//void WriteFieldLong(long pValue);

		///// <summary>
		///// WriteFieldDateTime writes a field with a DateTime value.
		///// </summary>
		///// <param name="pValue">DateTime value to write.</param>
		//void WriteFieldDateTime(DateTime pValue);

		///// <summary>
		///// WriteFieldBool writes a field with boolean value.
		///// </summary>
		///// <param name="pValue">Boolean value to write.</param>
		//void WriteFieldBool(bool pValue);

		///// <summary>
		///// WriteFieldDouble writes a field with double value.
		///// </summary>
		///// <param name="pValue">Double value to write.</param>
		//void WriteFieldDouble(double pValue);

		///// <summary>
		///// WriteFieldNull writes a field with null value.
		///// </summary>
		//void WriteFieldNull();
		*/
    }
}
