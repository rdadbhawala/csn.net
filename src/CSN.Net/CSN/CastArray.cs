// <copyright file="CastArray.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System;
	using System.Collections.Generic;
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
		public static implicit operator CastArray(long[] array)
		{
			return new Array<long>(FieldLong.F, array);
		}

		/// <summary>
		/// Cast array of Strings.
		/// </summary>
		/// <param name="array">Strings.</param>
		public static implicit operator CastArray(string[] array)
		{
			return new Array<string>(FieldString.F, array);
		}

		/// <summary>
		/// Cast array of Bools.
		/// </summary>
		/// <param name="array">Bools.</param>
		public static implicit operator CastArray(bool[] array)
		{
			return new Array<bool>(FieldBool.F, array);
		}

		/// <summary>
		/// Cast array of DateTimes.
		/// </summary>
		/// <param name="array">DateTimes.</param>
		public static implicit operator CastArray(DateTime[] array)
		{
			return new Array<DateTime>(FieldDateTime.F, array);
		}

		/// <summary>
		/// Cast array of doubles.
		/// </summary>
		/// <param name="array">doubles.</param>
		public static implicit operator CastArray(double[] array)
		{
			return new Array<double>(FieldDouble.F, array);
		}

		/// <summary>
		/// Cast array of References.
		/// </summary>
		/// <param name="array">References.</param>
		public static implicit operator CastArray(RecordCode[] array)
		{
			return new Array<int>(FieldReference.F, array.Select(x => x.SequenceNo));
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
