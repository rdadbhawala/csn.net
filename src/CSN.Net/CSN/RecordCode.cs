namespace Csn
{
	/// <summary>
	/// Record Code class.
	/// </summary>
    public class RecordCode
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RecordCode"/> class.
		/// </summary>
		/// <param name="rType">Record Type.</param>
		/// <param name="seqNo">Sequence Number.</param>
		public RecordCode(RecordType rType, long seqNo)
		{
			this.RecType = rType;
			this.SequenceNo = seqNo;
		}

		/// <summary>
		/// Gets the Record Type.
		/// </summary>
		public RecordType RecType { get; private set; }

		/// <summary>
		/// Gets the Sequence Number of the record.
		/// </summary>
		public long SequenceNo { get; private set; }
	}
}
