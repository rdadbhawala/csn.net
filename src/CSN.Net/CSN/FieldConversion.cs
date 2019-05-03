// <copyright file="FieldConversion.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Abstract implementation of Interface IField
	/// </summary>
	public abstract class FieldConversion : IField
	{
		/// <summary>
		/// Convert String to A Field
		/// </summary>
		/// <param name="s">String to convert</param>
		public static implicit operator FieldConversion(string s)
		{
			return new FieldString(s);
		}

		/// <summary>
		/// Write Field.
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		public abstract void WriteField(StreamWriter sw);
	}
}
