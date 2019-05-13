// <copyright file="IField.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// A CSN Field Handler for each Primitive Type.
	/// </summary>
	/// <typeparam name="TP">Primitive Type.</typeparam>
	internal interface IField<TP>
    {
		/// <summary>
		/// Writes this Field to the Stream.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		/// <param name="value">Value to write.</param>
		void WriteField(StreamWriter sw, TP value);
	}
}
