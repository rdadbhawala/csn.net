﻿// <copyright file="FieldBool.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Boolean Field Writer
	/// </summary>
	internal class FieldBool
		: FieldBase<bool>
    {
		/// <summary>
		/// Singleton FieldBool instance
		/// </summary>
		public static readonly FieldBool F = new FieldBool();

		private FieldBool()
			: base(Constants.ArrayCode.Bool)
		{
			// nothing
		}

		/// <summary>
		/// Write a boolean value to a CSN Field
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		/// <param name="value">Value to write</param>
		public override void WriteField(StreamWriter sw, bool value)
		{
			sw.Write(Constants.DefaultFieldSeparator);
			sw.Write(value ? Constants.BoolTrue : Constants.BoolFalse);
		}
    }
}
