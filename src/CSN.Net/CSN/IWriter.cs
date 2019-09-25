namespace Csn
{
	/// <summary>
	/// ICsnWriter is the raw CSN Writer.
	/// </summary>
	public interface IWriter : IWriterField
	{
		IWriter WriteTypeDef(string typeName, params string[] typeMembers);

		IWriterField WriteInstance(long refType);

		IWriterField WriteArray();
	}
}
