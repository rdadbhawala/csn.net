// <copyright file="ArrayLong.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// Array of Primitive Long
	/// </summary>
	internal class ArrayLong
		: CastArray, IValues
	{
		private readonly IEnumerable<long> values;

		/// <summary>
		/// Initializes a new instance of the <see cref="ArrayLong"/> class.
		/// </summary>
		/// <param name="arr">Array of Longs</param>
		public ArrayLong(IEnumerable<long> arr)
		{
			this.values = arr;
		}

		public override void WriteType(StreamWriter sw)
		{
			FieldLong.F.WriteType(sw);
		}

		/// <summary>
		/// Write Array
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		public override void WriteValues(StreamWriter sw)
		{
			FieldLong.F.WriteFields(sw, this.values);
		}
	}
}
