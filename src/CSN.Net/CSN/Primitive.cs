// <copyright file="Primitive.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System.IO;

	/// <summary>
	/// Common generic Primitive Type for individual values.
	/// </summary>
	/// <typeparam name="TP">Primitive Type.</typeparam>
	internal class Primitive<TP>
		: CastPrimitive
    {
		/// <summary>
		/// Field which can handle this type.
		/// </summary>
		private readonly IField<TP> field = null;

		/// <summary>
		/// Value.
		/// </summary>
		private readonly TP value = default(TP);

		/// <summary>
		/// Initializes a new instance of the <see cref="Primitive{P}"/> class.
		/// </summary>
		/// <param name="fld">IField for this type.</param>
		/// <param name="val">Value of this type.</param>
		public Primitive(IField<TP> fld, TP val)
		{
			this.field = fld;
			this.value = val;
		}

		/// <summary>
		/// Write the value.
		/// </summary>
		/// <param name="sw">Stream to write unto.</param>
		public override void WriteValue(StreamWriter sw)
		{
			this.field.WriteField(sw, this.value);
		}
	}
}
