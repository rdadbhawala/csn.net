// <copyright file="IField.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System;
	using System.IO;

	/// <summary>
	/// A CSN Field Handler for each Primitive Type
	/// </summary>
	public interface IField
    {
		/// <summary>
		/// Writes this Field to the Stream.
		/// </summary>
		/// <param name="sw">Stream to write unto</param>
		void WriteField(StreamWriter sw);
    }

	/// <summary>
	/// Field Handler for String primitive
	/// </summary>
	public class FieldString : FieldConversion, IField
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
					for (int charCtr = 0; charCtr < chars.Length; charCtr++)
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
