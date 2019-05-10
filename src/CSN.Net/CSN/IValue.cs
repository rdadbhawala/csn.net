// <copyright file="IValue.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Base interface for Primitive Types
	/// </summary>
	public interface IValue
    {
		/// <summary>
		/// Write the Primitive to the Stream
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		void WriteValue(StreamWriter sw);
    }
}
