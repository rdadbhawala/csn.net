﻿// <copyright file="IConfig.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	/// <summary>
	/// ICsnConfig declares CSN Configuration parameters.
	/// </summary>
	public interface IConfig
	{
		/// <summary>
		/// Gets the character used as a Field Separator.
		/// </summary>
		char FieldSeparator { get; }

		/// <summary>
		/// Gets the character used as a Record Separator.
		/// </summary>
		char RecordSeparator { get; }
	}
}