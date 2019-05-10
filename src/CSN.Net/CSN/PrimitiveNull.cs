// <copyright file="PrimitiveNull.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Primitive type for Null values
	/// </summary>
	public class PrimitiveNull
		: CastPrimitive, IValue
    {
		/// <summary>
		/// Singleton
		/// </summary>
		public static readonly PrimitiveNull Instance = new PrimitiveNull();

		private PrimitiveNull()
		{
			// nothing
		}

		/// <summary>
		/// Write a null field.
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		public override void WriteValue(StreamWriter sw)
		{
			FieldNull.F.WriteField(sw);
		}
	}
}
