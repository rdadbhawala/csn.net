// <copyright file="Writer.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System;
	using System.IO;

	/// <summary>
	/// CsnWriter is an implementation of ICsnWriter.
	/// </summary>
	public class Writer : IWriter, IFieldWriter
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
		/// Write a TypeDef Record.
		/// </summary>
		/// <param name="typeName">Type name.</param>
		/// <param name="typeMembers">Type Members.</param>
		/// <returns>Record Code.</returns>
		public RecordCode WriteTypeDefRecord(string typeName, params string[] typeMembers)
		{
			this.sw.Write(Constants.DefaultRecordSeparator);
			RecordCode rCode = this.WriteRecordCode(RecordType.TypeDef, Constants.RecordTypeChar.TypeDef);
			FieldString.F.WriteField(this.sw, typeName);
			FieldString.F.WriteFields(this.sw, typeMembers);
			return rCode;
		}

		/// <summary>
		/// Write an Instance Record.
		/// </summary>
		/// <param name="typeRecCode">Record Code of Instance Type.</param>
		/// <param name="values">Values of Instance.</param>
		/// <returns>Record Code.</returns>
		public RecordCode WriteInstanceRecord(RecordCode typeRecCode, params CastPrimitive[] values)
		{
			this.sw.Write(Constants.DefaultRecordSeparator);
			RecordCode rCode = this.WriteRecordCode(RecordType.Instance, Constants.RecordTypeChar.Instance);
			FieldReference.F.WriteField(this.sw, typeRecCode);
			for (int pCtr = 0; pCtr < values.Length; pCtr++)
			{
				values[pCtr].WriteValue(this.sw);
			}

			return rCode;
		}

		public IFieldWriter WriteInstanceFields(RecordCode typeRecCode, out RecordCode instance)
		{
			this.sw.Write(Constants.DefaultRecordSeparator);
			instance = this.WriteRecordCode(RecordType.Instance, Constants.RecordTypeChar.Instance);
			FieldReference.F.WriteField(this.sw, typeRecCode);
			return this;
		}

		/// <summary>
		/// Write an Array Record.
		/// </summary>
		/// <param name="values">Array values.</param>
		/// <returns>Record Code.</returns>
		public RecordCode WriteArrayRecord(CastArray values)
		{
			this.sw.Write(Constants.DefaultRecordSeparator);
			RecordCode rCode = this.WriteRecordCode(RecordType.Array, Constants.RecordTypeChar.Array);
			values.WriteType(this.sw);
			values.WriteValues(this.sw);
			return rCode;
		}

		/// <summary>
		/// Write an Array of Referneces.
		/// </summary>
		/// <param name="refType">Type of References.</param>
		/// <param name="arrayElements">Elements of Array; Instances of Type.</param>
		/// <returns>Record Code.</returns>
		public RecordCode WriteArrayRecord(RecordCode refType, RecordCode[] arrayElements)
		{
			this.sw.Write(Constants.DefaultRecordSeparator);
			RecordCode rCode = this.WriteRecordCode(RecordType.Array, Constants.RecordTypeChar.Array);
			FieldReference.F.WriteField(this.sw, refType);
			FieldReference.F.WriteFields(this.sw, arrayElements);
			return rCode;
		}

		private RecordCode WriteVersionRecord()
		{
			RecordCode currentRecordIndex = this.WriteRecordCode(RecordType.Version, Constants.RecordTypeChar.Version);
			FieldString.F.WriteField(this.sw, Constants.CsnVersion);
			return currentRecordIndex;
		}

		private RecordCode WriteRecordCode(RecordType recType, char rType)
		{
			RecordCode rCode = new RecordCode(recType, this.recordCounter++);
			this.sw.Write(rType);
			this.sw.Write(rCode.SequenceNo);
			return rCode;
		}

		public IFieldWriter W(string value)
		{
			FieldString.F.WriteField(this.sw, value);
			return this;
		}

		public IFieldWriter W(bool value)
		{
			FieldBool.F.WriteField(this.sw, value);
			return this;
		}

		public IFieldWriter W(DateTime value)
		{
			FieldDateTime.F.WriteField(this.sw, value);
			return this;
		}

		public IFieldWriter W(long value)
		{
			FieldLong.F.WriteField(this.sw, value);
			return this;
		}

		public IFieldWriter W(double value)
		{
			FieldDouble.F.WriteField(this.sw, value);
			return this;
		}

		public IFieldWriter W(RecordCode value)
		{
			FieldReference.F.WriteField(this.sw, value);
			return this;
		}
	}
}
