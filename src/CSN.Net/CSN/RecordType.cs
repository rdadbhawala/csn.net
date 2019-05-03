// <copyright file="RecordType.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	/// <summary>
	/// Record Type enumerates the type of CSN Records
	/// </summary>
    public enum RecordType
    {
		/// <summary>
		/// Version
		/// </summary>
		Version = 10,

		/// <summary>
		/// TypeDef
		/// </summary>
		TypeDef = 20,

		/// <summary>
		/// Array
		/// </summary>
		Array = 30,

		/// <summary>
		/// Instance
		/// </summary>
		Instance = 40
    }
}
