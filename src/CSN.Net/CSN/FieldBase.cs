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
	/// <typeparam name="T">Primitive Type.</typeparam>
	internal abstract class FieldBase<T>
		: IField<T>, IFields<T>
	{
		private readonly char arrType;

		/// <summary>
		/// Initializes a new instance of the <see cref="FieldBase{P}"/> class.
		/// </summary>
		/// <param name="pArrType">Array Type Character.</param>
		protected FieldBase(char pArrType)
		{
			this.arrType = pArrType;
		}

		/// <summary>
		/// Write an individual value to a CSN Field.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		/// <param name="value">Value to write.</param>
		public abstract void WriteField(StreamWriter sw, T value);

		/// <summary>
		/// Write a set of values to CSN Fields.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		/// <param name="values">Values to write.</param>
		public virtual void WriteFields(StreamWriter sw, IEnumerable<T> values)
		{
			foreach (T oneValue in values)
			{
				this.WriteField(sw, oneValue);
			}
		}

		/// <summary>
		/// Write the Array Type Code.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		public virtual void WriteType(StreamWriter sw)
		{
			sw.Write(Constants.FieldSeparator);
			sw.Write(Constants.Primitives.Prefix);
			sw.Write(this.arrType);
		}
	}
}
