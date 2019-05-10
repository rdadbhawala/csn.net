// <copyright file="CastArray.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;
	using System.Linq;

	/// <summary>
	/// Handle casting of Arrays of Primitives
	/// </summary>
	public abstract class CastArray : IValue
	{
		/// <summary>
		/// Cast array of Integers
		/// </summary>
		/// <param name="array">array</param>
		public static implicit operator CastArray(int[] array)
		{
			return new ArrayLong(array.Select(x => (long)x));
		}

		/// <summary>
		/// Write the Array
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		public abstract void WriteValue(StreamWriter sw);
	}
}
