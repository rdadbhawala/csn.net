// <copyright file="IFields.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// Field handler for Array Types.
	/// </summary>
	/// <typeparam name="TA">Array Type.</typeparam>
	internal interface IFields<TA>
    {
		/// <summary>
		/// Writes a set of values to CSN Fields.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		/// <param name="values">Values to write.</param>
		void WriteFields(StreamWriter sw, IEnumerable<TA> values);

		/// <summary>
		/// Write the Array Type Code.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		void WriteType(StreamWriter sw);
	}
}
