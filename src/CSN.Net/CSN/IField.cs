// <copyright file="IField.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// A CSN Field Handler for each Primitive Type
	/// </summary>
	/// <typeparam name="P">Primitive Type</typeparam>
	internal interface IField<P>
    {
		/// <summary>
		/// Writes this Field to the Stream.
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		/// <param name="value">Value to write.</param>
		void WriteField(StreamWriter sw, P value);

		/// <summary>
		/// Writes a set of values to CSN Fields.
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		/// <param name="values">Values to write</param>
		void WriteFields(StreamWriter sw, IEnumerable<P> values);

		void WriteType(StreamWriter sw);

	}
}
