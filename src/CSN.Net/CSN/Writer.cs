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
		/// Write the Version Record to the Stream.
		/// </summary>
		public void WriteVersionRecord()
		{
			this.WriteRecord(Constants.RecordTypeChar.Version, Constants.CsnVersion);
		}

		private void WriteRecord(char rType, params FieldConversion[] values)
		{
			this.WriteRecordCode(rType);
			if (values != null && values.Length > 0)
			{
				this.WriteField(values[0]);
			}
		}

		private void WriteField(IField v)
		{
			this.writer.Write(this.config.FieldSeparator);
			v.WriteField(this.writer);
		}

		private void WriteRecordCode(char rType)
		{
			this.writer.Write(rType);
			this.writer.Write(this.recordCounter);
			this.recordCounter++;
		}
	}
}
