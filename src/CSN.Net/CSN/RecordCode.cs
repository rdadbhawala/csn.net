// <copyright file="RecordCode.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	/// <summary>
	/// Record Code class
	/// </summary>
    public class RecordCode
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="RecordCode"/> class.
		/// </summary>
		/// <param name="rType">Record Type</param>
		/// <param name="seqNo">Sequence Number</param>
		internal RecordCode(RecordType rType, int seqNo)
		{
			this.RecType = rType;
			this.SequenceNo = seqNo;
		}

		/// <summary>
		/// Gets the Record Type
		/// </summary>
		public RecordType RecType { get; private set; }

		/// <summary>
		/// Gets the Sequence Number of the record
		/// </summary>
		public int SequenceNo { get; private set; }
	}
}
