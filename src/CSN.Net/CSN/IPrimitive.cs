// <copyright file="IPrimitive.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Base interface for Primitive Types
	/// </summary>
	public interface IPrimitive
    {
		/// <summary>
		/// Write the Primitive to the Stream
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		void WritePrimitive(StreamWriter sw);
    }
}
