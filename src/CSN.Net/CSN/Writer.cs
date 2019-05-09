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
		private readonly StreamWriter sw = null;
		private readonly Config config = null;
		private int recordCounter = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Writer"/> class.
		/// </summary>
		/// <param name="pStream">The IO Stream on which to write the CSN Payload.</param>
		/// <param name="pConfig">Configuration parameters for CSN.</param>
		public Writer(StreamWriter pStream, Config pConfig)
		{
			this.sw = pStream;
			this.config = pConfig;

			this.WriteVersionRecord();
		}

		/// <summary>
		/// Write a TypeDef Record
		/// </summary>
		/// <param name="typeName">Type name</param>
		/// <param name="typeMembers">Type Members</param>
		/// <returns>Record Code</returns>
		public RecordCode WriteTypeDefRecord(string typeName, params string[] typeMembers)
		{
			this.sw.Write(Constants.DefaultRecordSeparator);
			RecordCode currentRecordIndex = this.WriteRecordCode(RecordType.TypeDef, Constants.RecordTypeChar.TypeDef);
			this.WriteValue(typeName);
			this.WriteValues(typeMembers);
			return currentRecordIndex;
		}

		/// <summary>
		/// Write an Instance Record
		/// </summary>
		/// <param name="typeRecCode">Record Code of Instance Type</param>
		/// <param name="values">Values of Instance</param>
		/// <returns>Record Code</returns>
		public RecordCode WriteInstanceRecord(RecordCode typeRecCode, params PrimitiveCast[] values)
		{
			this.sw.Write(Constants.DefaultRecordSeparator);
			RecordCode currentRecordIndex = this.WriteRecordCode(RecordType.Instance, Constants.RecordTypeChar.Instance);
			FieldReference.R.WriteField(this.sw, typeRecCode.SequenceNo);
			for (int pCtr = 0; pCtr < values.Length; pCtr++)
			{
				values[pCtr].WritePrimitive(this.sw);
			}

			return currentRecordIndex;
		}

		private RecordCode WriteVersionRecord()
		{
			RecordCode currentRecordIndex = this.WriteRecordCode(RecordType.Version, Constants.RecordTypeChar.Version);
			this.WriteValue(Constants.CsnVersion);
			return currentRecordIndex;
		}

		private RecordCode WriteRecordCode(RecordType recType, char rType)
		{
			RecordCode rCode = new RecordCode(recType, this.recordCounter++);
			this.sw.Write(rType);
			this.sw.Write(rCode.SequenceNo);
			return rCode;
		}

		private void WriteValue(string str)
		{
			FieldString.W.WriteField(this.sw, str);
		}

		private void WriteValues(string[] arr)
		{
			FieldString.W.WriteFields(this.sw, arr);
		}
	}
}
