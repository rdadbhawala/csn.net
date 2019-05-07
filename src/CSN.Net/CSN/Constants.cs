// <copyright file="Constants.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	/// <summary>
	/// CSN Constants
	/// </summary>
    public static class Constants
    {
		/// <summary>
		/// Default Field Separator
		/// </summary>
		public const char DefaultFieldSeparator = ',';

		/// <summary>
		/// Default Record Separator
		/// </summary>
		public const char DefaultRecordSeparator = '\r';

		/// <summary>
		/// CSN Version String. This must align with the CSN Specification Version.
		/// </summary>
		public const string CsnVersion = "0.1.0";

		/// <summary>
		/// String Field Encloser (Double Quote) Character
		/// </summary>
		public const char StringFieldEncloser = '\"';

		/// <summary>
		/// Escape character for String values
		/// </summary>
		public const char StringEscapeChar = '\\';

		/// <summary>
		/// Reference Character Prefix
		/// </summary>
		public const char ReferencePrefix = '#';

		/// <summary>
		/// True value
		/// </summary>
		public const char BoolTrue = 'T';

		/// <summary>
		/// False value
		/// </summary>
		public const char BoolFalse = 'F';

		/// <summary>
		/// Record Type Characters
		/// </summary>
		public static class RecordTypeChar
		{
			/// <summary>
			/// Version
			/// </summary>
			public const char Version = 'V';

			/// <summary>
			/// TypeDef
			/// </summary>
			public const char TypeDef = 'T';

			/// <summary>
			/// Array
			/// </summary>
			public const char Array = 'A';

			/// <summary>
			/// Instance
			/// </summary>
			public const char Instance = 'I';
		}
	}
}
