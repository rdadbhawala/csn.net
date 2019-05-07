// <copyright file="PrimitiveBool.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Primitive Bool Wrapper
	/// </summary>
	public class PrimitiveBool
		: PrimitiveCast, IPrimitive
	{
		private readonly bool value;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimitiveBool"/> class.
		/// with given value
		/// </summary>
		/// <param name="b">boolean value</param>
		public PrimitiveBool(bool b)
		{
			this.value = b;
		}

		/// <summary>
		/// Writes a bool value to a CSN Field
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		public override void WritePrimitive(StreamWriter sw)
		{
			FieldBool.F.WriteField(sw, this.value);
		}
	}
}
