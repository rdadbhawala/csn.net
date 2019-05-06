// <copyright file="Writer.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System;
	using System.IO;

	/// <summary>
	/// CsnWriter is an implementation of ICsnWriter
	/// </summary>
	public class Writer : IWriter
	{
		// private readonly Stream stream = null;
		private readonly StreamWriter writer = null;
		private readonly Config config = null;
		private int recordCounter = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Writer"/> class.
		/// </summary>
		/// <param name="pStream">The IO Stream on which to write the CSN Payload.</param>
		/// <param name="pConfig">Configuration parameters for CSN.</param>
		public Writer(StreamWriter pStream, Config pConfig)
		{
			this.writer = pStream;
			this.config = pConfig;

			this.WriteVersionRecord();
		}

		/// <summary>
		/// Write a TypeDef Record
		/// </summary>
		/// <param name="typeName">Type name</param>
		/// <param name="typeMembers">Type Members</param>
		/// <returns>Record Sequence Number</returns>
		public int WriteTypeDefRecord(string typeName, params string[] typeMembers)
		{
			int currentRecordIndex = this.WriteRecordCode(Constants.RecordTypeChar.TypeDef);
			this.WriteValue(typeName);
			this.WriteValues(typeMembers);
			return currentRecordIndex;
		}

		private int WriteVersionRecord()
		{
			int currentRecordIndex = this.WriteRecordCode(Constants.RecordTypeChar.Version);
			this.WriteValue(Constants.CsnVersion);
			return currentRecordIndex;
		}

		private int WriteRecordCode(char rType)
		{
			int currentRecordIndex = this.recordCounter;
			this.writer.Write(rType);
			this.writer.Write(currentRecordIndex);
			this.recordCounter++;
			return currentRecordIndex;
		}

		private void WriteValue(string str)
		{
			FieldString.W.WriteField(this.writer, str);
		}

		private void WriteValues(string[] arr)
		{
			FieldString.W.WriteFields(this.writer, arr);
		}

		private void WriteField(IField v)
		{
			this.writer.Write(this.config.FieldSeparator);
			v.WriteField(this.writer);
		}
	}
}
