// <copyright file="IField.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// A CSN Field Handler for each Primitive Type
	/// </summary>
	public interface IField
    {
		/// <summary>
		/// Writes this Field to the Stream.
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		void WriteField(StreamWriter sw);
    }
}
