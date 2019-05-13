// <copyright file="Array.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// Generic Array Type.
	/// </summary>
	/// <typeparam name="TA">Array Element Type.</typeparam>
	internal class Array<TA>
		: CastArray
	{
		private readonly IFields<TA> fields = null;
		private readonly IEnumerable<TA> values = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="Array{A}"/> class.
		/// </summary>
		/// <param name="flds">Fields instance for Array Type.</param>
		/// <param name="vals">Values of Array.</param>
		public Array(IFields<TA> flds, IEnumerable<TA> vals)
		{
			this.fields = flds;
			this.values = vals;
		}

		/// <summary>
		/// Write Array Type.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		public override void WriteType(StreamWriter sw)
		{
			this.fields.WriteType(sw);
		}

		/// <summary>
		/// Write Array Values.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		public override void WriteValues(StreamWriter sw)
		{
			this.fields.WriteFields(sw, this.values);
		}
	}
}
