namespace Csn
{
	/// <summary>
	/// ICsnWriter is the raw CSN Writer.
	/// </summary>
	public interface IWriter : IWriterField
	{
		IWriter WriteTypeDef(string typeName, params string[] typeMembers);

		IWriterField WriteInstance(RecordCode typeRecCode);

		IWriterField WriteArray(PrimitiveType p);

		IWriter WriteArray(RecordCode refType); //, params RecordCode[] arrayElements);
	}
}
