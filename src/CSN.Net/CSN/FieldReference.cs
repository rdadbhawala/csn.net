// <copyright file="FieldReference.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Field Reference.
	/// </summary>
	internal class FieldReference
		: FieldBase<int>
    {
		/// <summary>
		/// Single FieldReference Instance.
		/// </summary>
		public static readonly FieldReference F = new FieldReference();

		private FieldReference()
			: base(Constants.ArrayCode.DateTime)
		{
			// nothing
		}

		/// <summary>
		/// Write a Reference Field.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		/// <param name="fieldValue">Value to write.</param>
		public override void WriteField(StreamWriter sw, int fieldValue)
		{
			sw.Write(Constants.DefaultFieldSeparator);
			sw.Write(Constants.ReferencePrefix);
			sw.Write(fieldValue);
		}
	}
}
