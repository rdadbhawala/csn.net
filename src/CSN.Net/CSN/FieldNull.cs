// <copyright file="FieldNull.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Null field
	/// </summary>
    internal class FieldNull
    {
		public static readonly FieldNull F = new FieldNull();

		private FieldNull()
		{
			// nothing
		}

		/// <summary>
		/// Write a null field
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		public void WriteField(StreamWriter sw)
		{
			sw.Write(Constants.DefaultFieldSeparator);
		}
    }
}
