// <copyright file="FieldString.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// FieldWriter for String Types
	/// </summary>
	internal class FieldString
		: FieldBase<string>
	{
		/// <summary>
		/// Singleton instance of FieldString
		/// </summary>
		public static readonly FieldString W = new FieldString();

		private FieldString()
			: base(Constants.ArrayCode.String)
		{
		}

		/// <summary>
		/// Write a value
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		/// <param name="fieldValue">Value to write</param>
		public override void WriteField(StreamWriter sw, string fieldValue)
		{
			sw.Write(Constants.DefaultFieldSeparator);
			if (fieldValue != null)
			{
				sw.Write(Constants.StringFieldEncloser);

				char[] chars = fieldValue.ToCharArray();
				if (chars.Length > 0)
				{
					this.WriteChar(chars[0], sw);
					for (int charCtr = 1; charCtr < chars.Length; charCtr++)
					{
						this.WriteChar(chars[charCtr], sw);
					}
				}

				sw.Write(Constants.StringFieldEncloser);
			}
		}

		private void WriteChar(char v, StreamWriter sw)
		{
			if (v == Constants.StringFieldEncloser || v == Constants.StringEscapeChar)
			{
				sw.Write(Constants.StringEscapeChar);
			}

			sw.Write(v);
		}
	}
}
