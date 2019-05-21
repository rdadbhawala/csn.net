// <copyright file="IWriter.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

using System;

namespace Abstraction.Csn
{
	/// <summary>
	/// ICsnWriter is the raw CSN Writer.
	/// </summary>
	public interface IWriter : IFieldWriter
	{
		/// <summary>
		/// Write a TypeDef Record.
		/// </summary>
		/// <param name="typeName">Type name.</param>
		/// <param name="typeMembers">Type Members.</param>
		/// <returns>Record Code.</returns>
		RecordCode WriteTypeDefRecord(string typeName, params string[] typeMembers);

		/// <summary>
		/// Write an Instance Record.
		/// </summary>
		/// <param name="typeRecCode">RecordCode of Instance Type.</param>
		/// <param name="values">Values of Instance.</param>
		/// <returns>Record Code.</returns>
		RecordCode WriteInstanceRecord(RecordCode typeRecCode, params CastPrimitive[] values);

		IFieldWriter WriteInstanceFields(RecordCode typeRecCode);

		/// <summary>
		/// Write an Array Record.
		/// </summary>
		/// <param name="values">Array values.</param>
		/// <returns>Record Code.</returns>
		RecordCode WriteArrayRecord(CastArray values);

		/// <summary>
		/// Write an Array of Referneces.
		/// </summary>
		/// <param name="refType">Type of References.</param>
		/// <param name="arrayElements">Elements of Array; Instances of Type.</param>
		/// <returns>Record Code.</returns>
		RecordCode WriteArrayRecord(RecordCode refType, RecordCode[] arrayElements);
	}
}
