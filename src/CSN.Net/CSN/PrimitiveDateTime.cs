// <copyright file="PrimitiveDateTime.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System;
	using System.IO;

	/// <summary>
	/// Primitive type for DateTime values
	/// </summary>
	internal class PrimitiveDateTime
		: CastPrimitive, IValue
	{
		private readonly DateTime value;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimitiveDateTime"/> class.
		/// with the given DateTime value.
		/// </summary>
		/// <param name="dt">Primitivei value</param>
		public PrimitiveDateTime(DateTime dt)
		{
			this.value = dt;
		}

		/// <summary>
		/// Write primitive to Stream
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		public override void WriteValue(StreamWriter sw)
		{
			FieldDateTime.F.WriteField(sw, this.value);
		}
	}
}
