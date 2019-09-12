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

		IWriterField WriteArray(PrimitiveType p);

		IWriter WriteArray(RecordCode refType); //, params RecordCode[] arrayElements);
	}
}
