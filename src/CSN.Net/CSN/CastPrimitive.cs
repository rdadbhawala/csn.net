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
		/// Convert String to A Primitive.
		/// </summary>
		/// <param name="s">String to convert.</param>
		public static implicit operator CastPrimitive(string s)
		{
			return new Primitive<string>(FieldString.F, s);
		}

		/// <summary>
		/// PrimitiveCast for DateTime values.
		/// </summary>
		/// <param name="dt">DateTime value to convert.</param>
		public static implicit operator CastPrimitive(DateTime dt)
		{
			return new Primitive<DateTime>(FieldDateTime.F, dt);
		}

		/// <summary>
		/// PrimitiveCast for double values.
		/// </summary>
		/// <param name="d">double value.</param>
		public static implicit operator CastPrimitive(double d)
		{
			return new Primitive<double>(FieldDouble.F, d);
		}

		/// <summary>
		/// Cast References to Primitives.
		/// </summary>
		/// <param name="r">RecordCode.</param>
		public static implicit operator CastPrimitive(RecordCode r)
		{
			return new Primitive<RecordCode>(FieldReference.F, r);
		}

		/// <summary>
		/// Write Primitive.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		public abstract void WriteValue(StreamWriter sw);
	}
}
