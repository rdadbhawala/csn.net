// <copyright file="Config.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	/// <summary>
	/// CsnConfig declares CSN Configuration parameters.
	/// </summary>
    public class Config : IConfig
    {
		/// <summary>
		/// Gets or sets a character used as a Field Separator.
		/// </summary>
		public char FieldSeparator { get; set; }

		/// <summary>
		/// Gets or sets a character used as a Record Separator.
		/// </summary>
		public char RecordSeparator { get; set; }

		/// <summary>
		/// Create a new instance of a default CSN Configuration.
		/// </summary>
		/// <returns>An instance of CsnConfig.</returns>
		public static Config CreateDefaultConfig()
		{
			return new Config
			{
				FieldSeparator = Constants.DefaultFieldSeparator,
				RecordSeparator = Constants.DefaultRecordSeparator,
			};
		}
	}
}
