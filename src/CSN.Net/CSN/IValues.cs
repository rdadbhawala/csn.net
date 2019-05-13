// <copyright file="IValues.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Array Values.
	/// </summary>
	public interface IValues
    {
		/// <summary>
		/// Write Array of Values.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		void WriteValues(StreamWriter sw);

		/// <summary>
		/// Write Array Type Code.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		void WriteType(StreamWriter sw);
    }
}
