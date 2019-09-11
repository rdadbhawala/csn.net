// <copyright file="CastPrimitive.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System;
	using System.IO;

	/// <summary>
	/// Abstract implementation of Interface IPrimitive for casting convenience.
	/// </summary>
	public abstract class CastPrimitive : IValue
	{
		/// <summary>
		/// Write Primitive.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		public abstract void WriteValue(StreamWriter sw);
	}
}
