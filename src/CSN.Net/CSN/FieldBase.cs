// <copyright file="FieldBase.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// Base class for Fields, abstracting the Array implementation.
	/// </summary>
	/// <typeparam name="P">Primitive Type</typeparam>
	internal abstract class FieldBase<P>
		: IField<P>
	{
		public readonly char ArrType;

		protected FieldBase(char pArrType)
		{
			this.ArrType = pArrType;
		}

		/// <summary>
		/// Write an individual value to a CSN Field.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		/// <param name="value">Value to write.</param>
		public abstract void WriteField(StreamWriter sw, P value);

		/// <summary>
		/// Write a set of values to CSN Fields.
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		/// <param name="values">Values to write</param>
		public virtual void WriteFields(StreamWriter sw, IEnumerable<P> values)
		{
			foreach (P oneValue in values)
			{
				this.WriteField(sw, oneValue);
			}
		}

		public virtual void WriteType(StreamWriter sw)
		{
			sw.Write(Constants.DefaultFieldSeparator);
			sw.Write(Constants.ArrayCode.Prefix);
			sw.Write(this.ArrType);
		}
	}
}
