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
	{
		/// <summary>
		/// Singleton instance of FieldString
		/// </summary>
		public static readonly FieldString W = new FieldString();

		private FieldString()
		{
		}

		/// <summary>
		/// Write many field values.
		/// </summary>
		/// <param name="sw">Writer to write unto</param>
		/// <param name="fieldValues">Values to write</param>
		public void WriteFields(StreamWriter sw, string[] fieldValues)
		{
			for (int ctr = 0; ctr < fieldValues.Length; ctr++)
			{
				this.WriteField(sw, fieldValues[ctr]);
			}
		}

		/// <summary>
		/// Write a value
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		/// <param name="fieldValue">Value to write</param>
		public void WriteField(StreamWriter sw, string fieldValue)
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

	/*
	/// <summary>
	/// Field Handler for String primitive
	/// </summary>
	class FieldString : FieldConversion, IField
	{
		private readonly string fieldValue = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="FieldString"/> class.
		/// from a String literal.
		/// </summary>
		/// <param name="value">value to write</param>
		public FieldString(string value)
		{
			this.fieldValue = value;
		}

		/// <summary>
		/// Convert String to FieldString
		/// </summary>
		/// <param name="s">String to convert</param>
		public static implicit operator FieldString(string s)
		{
			return new FieldString(s);
		}

		/// <summary>
		/// Writes String to the Stream
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		public override void WriteField(StreamWriter sw)
		{
			if (this.fieldValue != null)
			{
				sw.Write(Constants.StringFieldEncloser);

				char[] chars = this.fieldValue.ToCharArray();
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
	*/
}
