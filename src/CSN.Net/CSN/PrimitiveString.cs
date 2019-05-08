// <copyright file="PrimitiveString.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// String value Primitive wrapper class
	/// </summary>
	internal class PrimitiveString
		: PrimitiveCast, IPrimitive
	{
		private readonly string value = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrimitiveString"/> class.
		/// with given value.
		/// </summary>
		/// <param name="str">value</param>
		public PrimitiveString(string str)
		{
			this.value = str;
		}

		/// <summary>
		/// Write the Primitive to a Field.
		/// </summary>
		/// <param name="sw">Value to write.</param>
		public override void WritePrimitive(StreamWriter sw)
		{
			FieldString.W.WriteField(sw, this.value);
		}
	}
}
