// <copyright file="FieldDouble.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Double Field Writer.
	/// </summary>
	internal class FieldDouble
		: FieldBase<double>
    {
		/// <summary>
		/// Singleton FieldDouble instance.
		/// </summary>
		public static readonly FieldDouble F = new FieldDouble();

		private FieldDouble()
			: base(Constants.Primitives.Real)
		{
			// nothing
		}

		/// <summary>
		/// Write a Double value to a CSN Field.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		/// <param name="value">Value to write.</param>
		public override void WriteField(StreamWriter sw, double value)
		{
			sw.Write(Constants.FieldSeparator);
			sw.Write(value);
		}
    }
}
