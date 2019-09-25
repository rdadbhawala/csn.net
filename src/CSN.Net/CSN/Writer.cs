namespace Csn
{
	using System;
	using System.IO;

	/// <summary>
	/// CsnWriter is an implementation of ICsnWriter.
	/// </summary>
	public partial class Writer : IWriter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Writer"/> class.
		/// </summary>
		/// <param name="pStream">The IO Stream on which to write the CSN Payload.</param>
		/// <param name="pConfig">Configuration parameters for CSN.</param>
		public Writer(StreamWriter pStream)
		{
			this.mCurrent = -1;
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
			this.WriteNewRecord();
			this.W(typeName);
			W(typeMembers);
			return this;
		}

		public IWriterField WriteInstance(long refTypeSeqNo)
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteNewRecord();
			WRef(refTypeSeqNo);
			return this;
		}

		public IWriterField WriteArray()
		{
			this.sw.Write(Constants.RecordSeparator);
			this.WriteNewRecord();
			this.sw.Write(Constants.FieldSeparator);
			this.sw.Write(Constants.RecordTypeChar.Array);
			return this;
		}

		//private void WritePrimitiveType(PrimitiveType p)
		//{
		//	sw.Write(Constants.FieldSeparator);
		//	sw.Write(Constants.Primitives.Prefix);
		//	switch (p)
		//	{
		//		case PrimitiveType.Bool:
		//			sw.Write(Constants.Primitives.Bool);
		//			break;
		//		case PrimitiveType.DateTime:
		//			sw.Write(Constants.Primitives.DateTime);
		//			break;
		//		case PrimitiveType.Int:
		//			sw.Write(Constants.Primitives.Integer);
		//			break;
		//		case PrimitiveType.Real:
		//			sw.Write(Constants.Primitives.Real);
		//			break;
		//		case PrimitiveType.String:
		//			sw.Write(Constants.Primitives.String);
		//			break;
		//	}
		//}

		//public IWriter WriteArray(long refType)
		//{
		//	this.sw.Write(Constants.RecordSeparator);
		//	this.WriteNewRecord(RecordType.Array, Constants.RecordTypeChar.Array);
		//	WRef(refType);
		//	return this;
		//}

		private void WriteVersionRecord()
		{
			this.WriteNewRecord();
			this.W(Constants.CsnVersion);
		}

		private void WriteNewRecord()
		{
			this.sw.Write(++this.mCurrent);
		}
	}
}
