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
			this.WriteNewRecord(RecordType.TypeDef, Constants.RecordTypeChar.TypeDef);
			FieldString.F.WriteField(this.sw, typeName);
			FieldString.F.WriteFields(this.sw, typeMembers);
			return this;
		}

		public IFieldWriter WriteInstanceRecord(RecordCode typeRef)
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteNewRecord(RecordType.Instance, Constants.RecordTypeChar.Instance);
			this.WriteRecordCode(typeRef);
			return this;
		}

		public IFieldWriter WriteInstanceFields(RecordCode typeRecCode)
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteNewRecord(RecordType.Instance, Constants.RecordTypeChar.Instance);
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
			this.WriteNewRecord(RecordType.Array, Constants.RecordTypeChar.Array);
			values.WriteType(this.sw);
			values.WriteValues(this.sw);
			return this;
		}

		public IFieldWriter WriteArrayPrimitives(PrimitiveType p)
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteNewRecord(RecordType.Array, Constants.RecordTypeChar.Array);
			WritePrimitiveType(p);
			return this;
		}

		private void WritePrimitiveType(PrimitiveType p)
		{
			sw.Write(Constants.FieldSeparator);
			sw.Write(Constants.Primitives.Prefix);
			switch (p)
			{
				case PrimitiveType.Bool:
					sw.Write(Constants.Primitives.Bool);
					break;
				case PrimitiveType.DateTime:
					sw.Write(Constants.Primitives.DateTime);
					break;
				case PrimitiveType.Int:
					sw.Write(Constants.Primitives.Integer);
					break;
				case PrimitiveType.Real:
					sw.Write(Constants.Primitives.Real);
					break;
				case PrimitiveType.String:
					sw.Write(Constants.Primitives.String);
					break;
			}
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
			this.WriteNewRecord(RecordType.Array, Constants.RecordTypeChar.Array);
			FieldReference.F.WriteField(this.sw, refType);
			FieldReference.F.WriteFields(this.sw, arrayElements);
			return this;
		}

		private void WriteVersionRecord()
		{
			this.WriteNewRecord(RecordType.Version, Constants.RecordTypeChar.Version);
			FieldString.F.WriteField(this.sw, Constants.CsnVersion);
		}

		private void WriteNewRecord(RecordType recType, char rType)
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
			sw.Write(Constants.FieldSeparator);
			sw.Write(value ? Constants.BoolTrue : Constants.BoolFalse);
			return this;
		}

		public IFieldWriter W(bool[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				W(values[i]);
			}
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
			WriteRecordCode(value);
			return this;
		}

		protected void WriteRecordCode(RecordCode value)
		{
			if (value == null)
			{
				sw.Write(Constants.FieldSeparator);
			}
			else
			{
				sw.Write(Constants.FieldSeparator);
				sw.Write(Constants.ReferencePrefix);
				sw.Write(value.SequenceNo);
			}
		}
	}
}
