// <copyright file="CastArray.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;
	using System.Linq;

	/// <summary>
	/// Handle casting of Arrays of Primitives.
	/// </summary>
	public abstract class CastArray : IValues
	{
		/// <summary>
		/// Cast array of Integers.
		/// </summary>
		/// <param name="array">array.</param>
		public static implicit operator CastArray(int[] array)
		{
			return new Array<long>(FieldLong.F, array.Select(x => (long)x));
		}

		/// <summary>
		/// Write the Array.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		public abstract void WriteValues(StreamWriter sw);

		/// <summary>
		/// Write the Array Type Code.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		public abstract void WriteType(StreamWriter sw);
	}
}
