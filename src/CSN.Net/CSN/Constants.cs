// <copyright file="Constants.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

using System;

namespace Abstraction.Csn
{
	/// <summary>
	/// CSN Constants.
	/// </summary>
    public static class Constants
    {
		/// <summary>
		/// Default Field Separator.
		/// </summary>
		public const char FieldSeparator = ',';

		/// <summary>
		/// Default Record Separator.
		/// </summary>
		public const char RecordSeparator = '\n';

		/// <summary>
		/// CSN Version String. This must align with the CSN Specification Version.
		/// </summary>
		public const string CsnVersion = "0.1.0";

		/// <summary>
		/// String Field Encloser (Double Quote) Character.
		/// </summary>
		public const char StringFieldEncloser = '\"';

		/// <summary>
		/// Escape character for String values.
		/// </summary>
		public const char StringEscapeChar = '\\';

		/// <summary>
		/// Reference Character Prefix.
		/// </summary>
		public const char ReferencePrefix = '#';

		/// <summary>
		/// True value.
		/// </summary>
		public const char BoolTrue = 'T';

		/// <summary>
		/// False value.
		/// </summary>
		public const char BoolFalse = 'F';

		/// <summary>
		/// DateTime field prefix.
		/// </summary>
		public const char DateTimePrefix = 'D';
		public const char DateTimeT = 'T';

		/// <summary>
		/// Record Type Characters.
		/// </summary>
		public static class RecordTypeChar
		{
			/// <summary>
			/// Version.
			/// </summary>
			public const char Version = 'V';

			/// <summary>
			/// TypeDef.
			/// </summary>
			public const char TypeDef = 'T';

			/// <summary>
			/// Array.
			/// </summary>
			public const char Array = 'A';

			/// <summary>
			/// Instance.
			/// </summary>
			public const char Instance = 'I';
		}

		/// <summary>
		/// Array Primitive Char Codes.
		/// </summary>
		public static class Primitives
		{
			/// <summary>
			/// Prefix Character.
			/// </summary>
			public const char Prefix = 'P';

			/// <summary>
			/// Integer type.
			/// </summary>
			public const char Integer = 'I';

			/// <summary>
			/// Bool type.
			/// </summary>
			public const char Bool = 'B';

			/// <summary>
			/// String type.
			/// </summary>
			public const char String = 'S';

			/// <summary>
			/// Real number type.
			/// </summary>
			public const char Real = 'R';

			/// <summary>
			/// DateTime type.
			/// </summary>
			public const char DateTime = 'D';
		}
	}
}
