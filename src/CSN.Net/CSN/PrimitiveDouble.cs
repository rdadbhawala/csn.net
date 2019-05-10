// <copyright file="PrimitiveDouble.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Primitive Double Wrapper
	/// </summary>
	internal class PrimitiveDouble
		: CastPrimitive, IValue
	{
		private readonly double value;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimitiveDouble"/> class.
		/// with given value
		/// </summary>
		/// <param name="pValue">double value</param>
		public PrimitiveDouble(double pValue)
		{
			this.value = pValue;
		}

		/// <summary>
		/// Writes a double value to a CSN Field
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		public override void WriteValue(StreamWriter sw)
		{
			FieldDouble.F.WriteField(sw, this.value);
		}
	}
}
