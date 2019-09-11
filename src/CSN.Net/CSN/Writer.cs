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
	public partial class Writer : IWriter
	{
		private long recordCounter = -1;

		/// <summary>
		/// Initializes a new instance of the <see cref="Writer"/> class.
		/// </summary>
		/// <param name="pStream">The IO Stream on which to write the CSN Payload.</param>
		/// <param name="pConfig">Configuration parameters for CSN.</param>
		public Writer(StreamWriter pStream)
		{
			this.sw = pStream;
			this.WriteVersionRecord();
		}


		/// <summary>
		/// Write a TypeDef Record.
		/// </summary>
		/// <param name="typeName">Type name.</param>
		/// <param name="typeMembers">Type Members.</param>
		/// <returns>Record Code.</returns>
		public IWriter WriteTypeDef(string typeName, params string[] typeMembers)
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteNewRecord(RecordType.TypeDef, Constants.RecordTypeChar.TypeDef);
			this.W(typeName);
			W(typeMembers);
			return this;
		}

		public IWriterField WriteInstance(RecordCode typeRecCode)
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteNewRecord(RecordType.Instance, Constants.RecordTypeChar.Instance);
			W(typeRecCode);
			return this;
		}

		public IWriterField WriteArrayPrimitives(PrimitiveType p)
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
		public IWriter WriteArrayRefs(RecordCode refType, params RecordCode[] arrayElements)
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteNewRecord(RecordType.Array, Constants.RecordTypeChar.Array);
			W(refType);
			W(arrayElements);
			return this;
		}

		private void WriteVersionRecord()
		{
			this.WriteNewRecord(RecordType.Version, Constants.RecordTypeChar.Version);
			this.W(Constants.CsnVersion);
		}

		private void WriteNewRecord(RecordType recType, char rType)
		{
			this.recordCounter++;
			this.sw.Write(rType);
			this.sw.Write(this.recordCounter);
			this.Current = new RecordCode(recType, this.recordCounter);
		}

	}
}
