// <copyright file="CastArray.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	/// <summary>
	/// Handle casting of Arrays of Primitives.
	/// </summary>
	public abstract class CastArray : IValues
	{
		/// <summary>
		/// Write the Array.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		public abstract void WriteValues(StreamWriter sw);

		/// <summary>
		/// Write the Array Type Code.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		public abstract void WriteType(StreamWriter sw);
	}
}
