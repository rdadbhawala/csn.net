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
		/// PrimitiveCast for DateTime values.
		/// </summary>
		/// <param name="dt">DateTime value to convert.</param>
		public static implicit operator CastPrimitive(DateTime dt)
		{
			return new Primitive<DateTime>(FieldDateTime.F, dt);
		}

		/// <summary>
		/// Write Primitive.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		public abstract void WriteValue(StreamWriter sw);
	}
}
