// <copyright file="PrimitiveCast.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System;
	using System.IO;

	/// <summary>
	/// Abstract implementation of Interface IPrimitive for casting convenience
	/// </summary>
	public abstract class PrimitiveCast : IPrimitive
	{
		/// <summary>
		/// Convert String to A Primitive
		/// </summary>
		/// <param name="s">String to convert</param>
		public static implicit operator PrimitiveCast(string s)
		{
			return new PrimitiveString(s);
		}

		/// <summary>
		/// Convert a boolean to a Primitive
		/// </summary>
		/// <param name="b">boolean to convert</param>
		public static implicit operator PrimitiveCast(bool b)
		{
			return new PrimitiveBool(b);
		}

		/// <summary>
		/// PrimitiveCast for DateTime values
		/// </summary>
		/// <param name="dt">DateTime value to convert</param>
		public static implicit operator PrimitiveCast(DateTime dt)
		{
			return new PrimitiveDateTime(dt);
		}

		/// <summary>
		/// Write Primitive.
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		public abstract void WritePrimitive(StreamWriter sw);
	}
}
