// <copyright file="IWriter.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

using System;

namespace Abstraction.Csn
{
	/// <summary>
	/// ICsnWriter is the raw CSN Writer.
	/// </summary>
	public interface IWriter : IWriterField
	{
		IWriter WriteTypeDef(string typeName, params string[] typeMembers);

		IWriterField WriteInstance(RecordCode typeRecCode);

		IWriterField WriteArrayPrimitives(PrimitiveType p);

		IWriter WriteArrayRefs(RecordCode refType, params RecordCode[] arrayElements);
	}
}
