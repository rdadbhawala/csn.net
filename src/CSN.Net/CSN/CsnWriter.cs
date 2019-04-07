// <copyright file="CsnWriter.cs" company="Abstraction">
// Copyright (c) Abstraction. All rights reserved.
// </copyright>

namespace Abstraction.Csn
{
	using System;
	using System.IO;

	/// <summary>
	/// CsnWriter is an implementation of ICsnWriter
	/// </summary>
	public class CsnWriter
	{
		private readonly Stream stream = null;
		private readonly CsnConfig config = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="CsnWriter"/> class.
		/// </summary>
		/// <param name="pStream">The IO Stream on which to write the CSN Payload.</param>
		/// <param name="pConfig">Configuration parameters for CSN.</param>
		public CsnWriter(Stream pStream, CsnConfig pConfig)
		{
			this.stream = pStream;
			this.config = pConfig;
		}
	}
}
