// <copyright file="RecordType.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	/// <summary>
	/// Record Type enumerates the type of CSN Records.
	/// </summary>
    public enum RecordType
    {
		Unknown = 0,

		/// <summary>
		/// Version.
		/// </summary>
		Version = 10,

		/// <summary>
		/// TypeDef.
		/// </summary>
		TypeDef = 20,

		/// <summary>
		/// Array.
		/// </summary>
		Array = 30,

		/// <summary>
		/// Instance.
		/// </summary>
		Instance = 40,
    }

	public enum PrimitiveType
	{
		Unknown = 0,
		Bool = 10,
		Int = 20,
		Real = 30,
		DateTime = 40,
		String = 50
	}
}
