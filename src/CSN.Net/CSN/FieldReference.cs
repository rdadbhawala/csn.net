// <copyright file="FieldReference.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Field Reference
	/// </summary>
	internal class FieldReference
    {
		/// <summary>
		/// Single FieldReference Instance
		/// </summary>
		public static readonly FieldReference R = new FieldReference();

		private FieldReference()
		{
			// nothing
		}

		/// <summary>
		/// Write many References.
		/// </summary>
		/// <param name="sw">Writer to write unto</param>
		/// <param name="fieldValues">Values to write</param>
		public void WriteFields(StreamWriter sw, int[] fieldValues)
		{
			for (int ctr = 0; ctr < fieldValues.Length; ctr++)
			{
				this.WriteField(sw, fieldValues[ctr]);
			}
		}

		/// <summary>
		/// Write a Reference Field.
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		/// <param name="fieldValue">Value to write.</param>
		public void WriteField(StreamWriter sw, int fieldValue)
		{
			sw.Write(Constants.DefaultFieldSeparator);
			sw.Write(Constants.ReferencePrefix);
			sw.Write(fieldValue);
		}
	}
}
