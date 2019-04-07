// <copyright file="CsnConfig.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	/// <summary>
	/// CsnConfig declares CSN Configuration parameters.
	/// </summary>
    public class CsnConfig
    {
		/// <summary>
		/// Gets or sets a character used as a Field Separator.
		/// </summary>
		public char FieldSeparator { get; set; }

		/// <summary>
		/// Gets or sets a character used as a Record Separator.
		/// </summary>
		public char RecordSeparator { get; set; }
	}
}
