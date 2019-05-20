// <copyright file="FieldReference.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// Field Reference.
	/// </summary>
	internal class FieldReference
		: IField<RecordCode>, IFields<RecordCode>
    {
		/// <summary>
		/// Single FieldReference Instance.
		/// </summary>
		public static readonly FieldReference F = new FieldReference();

		private FieldReference()
		{
			// nothing
		}

		/// <summary>
		/// Write a Reference Field.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		/// <param name="fieldValue">Value to write.</param>
		public void WriteField(StreamWriter sw, RecordCode fieldValue)
		{
			if (fieldValue == null)
			{
				FieldNull.F.WriteField(sw);
			}
			else
			{
				sw.Write(Constants.DefaultFieldSeparator);
				sw.Write(Constants.ReferencePrefix);
				sw.Write(fieldValue.SequenceNo);
			}
		}

		/// <summary>
		/// Write an array of References.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		/// <param name="values">Values to write.</param>
		public void WriteFields(StreamWriter sw, IEnumerable<RecordCode> values)
		{
			foreach (RecordCode oneValue in values)
			{
				this.WriteField(sw, oneValue);
			}
		}

		/// <summary>
		/// Array Code for References. This method should not be invoked.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		public void WriteType(StreamWriter sw)
		{
			// this method should not be invoked
			throw new System.NotImplementedException();
		}
	}
}
