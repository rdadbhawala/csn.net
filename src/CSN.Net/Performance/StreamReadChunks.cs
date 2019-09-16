using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Performance
{
    public class StreamReadChunks
    {
		MemoryStream stream = null;
		readonly int ctr = 1000000;
		StreamReader reader = null;
		readonly int readSize = 1000;

		[IterationSetup]
		public void Setup()
		{
			stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
			for (int i = 0; i < ctr; i++)
			{
				writer.Write('a');
			}
			writer.Flush();
			stream.Position = 0;
			reader = new StreamReader(stream, Encoding.UTF8);
		}

		[Benchmark]
		public void ReadOne()
		{
			int readChar = -1;
			char ch = 'a';
			while ((readChar = reader.Read()) != -1)
			{
				ch = ((char)readChar);
			}
		}

		[Benchmark]
		public void Read1000()
		{
			char[] readBlock = new char[readSize];
			int readLen = 0;
			char ch = 'a';
			while ((readLen = reader.Read(readBlock, 0, readSize)) > 0)
			{
				for (int i = 0; i < readLen; i++)
				{
					ch = readBlock[i];
				}
			}
		}
    }
}
