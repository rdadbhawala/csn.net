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
		IWriter WriteTypeDefRecord(string typeName, params string[] typeMembers);

		IFieldWriter WriteInstanceRecord(RecordCode typeRef);

		IFieldWriter WriteInstanceFields(RecordCode typeRecCode);

		/// <summary>
		/// Write an Array Record.
		/// </summary>
		/// <param name="values">Array values.</param>
		/// <returns>Record Code.</returns>
		IWriter WriteArrayRecord(CastArray values);

		/// <summary>
		/// Write an Array of Referneces.
		/// </summary>
		/// <param name="refType">Type of References.</param>
		/// <param name="arrayElements">Elements of Array; Instances of Type.</param>
		/// <returns>Record Code.</returns>
		IWriter WriteArrayRecord(RecordCode refType, RecordCode[] arrayElements);
	}
}
