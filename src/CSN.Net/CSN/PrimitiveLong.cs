// <copyright file="PrimitiveLong.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Primitive Long Wrapper
	/// </summary>
	internal class PrimitiveLong
		: CastPrimitive, IValue
	{
		private readonly long value;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimitiveLong"/> class.
		/// with given value
		/// </summary>
		/// <param name="l">long value</param>
		public PrimitiveLong(long l)
		{
			this.value = l;
		}

		/// <summary>
		/// Writes a long value to a CSN Field
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		public override void WriteValue(StreamWriter sw)
		{
			FieldLong.F.WriteField(sw, this.value);
		}
	}
}
