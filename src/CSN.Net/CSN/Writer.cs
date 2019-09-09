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
	public class Writer : IWriter
	{
		// private readonly Stream stream = null;
		private readonly StreamWriter sw = null;
		//private readonly Config config = null;
		private long recordCounter = -1;

		/// <summary>
		/// Initializes a new instance of the <see cref="Writer"/> class.
		/// </summary>
		/// <param name="pStream">The IO Stream on which to write the CSN Payload.</param>
		/// <param name="pConfig">Configuration parameters for CSN.</param>
		public Writer(StreamWriter pStream)
		{
			this.sw = pStream;
			//this.config = pConfig;

			this.WriteVersionRecord();
		}

		public RecordCode Current { get; private set; }

		/// <summary>
		/// Write a TypeDef Record.
		/// </summary>
		/// <param name="typeName">Type name.</param>
		/// <param name="typeMembers">Type Members.</param>
		/// <returns>Record Code.</returns>
		public IWriter WriteTypeDefRecord(string typeName, params string[] typeMembers)
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteRecordCode(RecordType.TypeDef, Constants.RecordTypeChar.TypeDef);
			FieldString.F.WriteField(this.sw, typeName);
			FieldString.F.WriteFields(this.sw, typeMembers);
			return this;
		}

		/// <summary>
		/// Write an Instance Record.
		/// </summary>
		/// <param name="typeRecCode">Record Code of Instance Type.</param>
		/// <param name="values">Values of Instance.</param>
		/// <returns>Record Code.</returns>
		public IWriter WriteInstanceRecord(RecordCode typeRecCode, params CastPrimitive[] values)
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteRecordCode(RecordType.Instance, Constants.RecordTypeChar.Instance);
			FieldReference.F.WriteField(this.sw, typeRecCode);
			for (int pCtr = 0; pCtr < values.Length; pCtr++)
			{
				values[pCtr].WriteValue(this.sw);
			}

			return this;
		}

		public IFieldWriter WriteInstanceFields(RecordCode typeRecCode)
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteRecordCode(RecordType.Instance, Constants.RecordTypeChar.Instance);
			FieldReference.F.WriteField(this.sw, typeRecCode);
			return this;
		}

		/// <summary>
		/// Write an Array Record.
		/// </summary>
		/// <param name="values">Array values.</param>
		/// <returns>Record Code.</returns>
		public IWriter WriteArrayRecord(CastArray values)
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteRecordCode(RecordType.Array, Constants.RecordTypeChar.Array);
			values.WriteType(this.sw);
			values.WriteValues(this.sw);
			return this;
		}

		/// <summary>
		/// Write an Array of Referneces.
		/// </summary>
		/// <param name="refType">Type of References.</param>
		/// <param name="arrayElements">Elements of Array; Instances of Type.</param>
		/// <returns>Record Code.</returns>
		public IWriter WriteArrayRecord(RecordCode refType, RecordCode[] arrayElements)
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteRecordCode(RecordType.Array, Constants.RecordTypeChar.Array);
			FieldReference.F.WriteField(this.sw, refType);
			FieldReference.F.WriteFields(this.sw, arrayElements);
			return this;
		}

		private void WriteVersionRecord()
		{
			this.WriteRecordCode(RecordType.Version, Constants.RecordTypeChar.Version);
			FieldString.F.WriteField(this.sw, Constants.CsnVersion);
		}

		private void WriteRecordCode(RecordType recType, char rType)
		{
			this.recordCounter++;
			this.sw.Write(rType);
			this.sw.Write(this.recordCounter);
			this.Current = new RecordCode(recType, this.recordCounter);
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
