using Csn;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace Performance
{
	[TestClass]
	public class LongCheck
	{
		public static long ctr = 10000;
		static LongCheck()
		{ }

		[TestMethod]
		public void Program()
		{
			LongDeser deser = new LongDeser();
			deser.Setup();
			deser.LongDeserDo();
		}

		private static Stream GetStream()
		{
			return new MemoryStream();
		}

		public class LongSer
		{
			StreamWriter sw = null;
			Writer w = null;
			internal Stream tgt = LongCheck.GetStream();

			public void Setup()
			{
				sw = new StreamWriter(tgt, Encoding.UTF8);
				w = new Writer(sw);
			}

			public void LongSerDo()
			{
				IWriterField fw = w.WriteArray();
				for (long i = -LongCheck.ctr; i < LongCheck.ctr; i++)
				{
					fw.W(i);
				}
				sw.Flush();
			}
		}

		public class LongDeser
		{
			Stream mStream = new MemoryStream();
			StreamReader sr = null;
			ReadRec rr = null;

			public LongDeser()
			{
				LongSer ser = new LongSer
				{
					tgt = mStream
				};
				ser.Setup();
				ser.LongSerDo();
			}

			public void Setup()
			{
				mStream.Position = 0;
				sr = new StreamReader(mStream, Encoding.UTF8);
				rr = new ReadRec();
			}

			public void LongDeserDo()
			{
				Reader.Singleton.Read(sr, rr);
			}

			class ReadRec : IRead, IReadValue
			{
				public IReadValue GetReadValue()
				{
					return this;
				}

				public void Read(VersionRecord verRec)
				{
				}

				public void Read(TypeDefRecord typeRec)
				{
					throw new InvalidOperationException();
				}

				public void Read(InstanceRecord instRec)
				{
					throw new InvalidOperationException();
				}

				public void Read(ArrayRecord arrRec)
				{ }

				public void ReadValueNull(ValueRecord rec, int index)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, bool value)
				{
					throw new InvalidOperationException();
				}

				long ctr = -LongCheck.ctr;
				public void ReadValue(ValueRecord rec, int index, long value)
				{
					Assert.AreEqual(ctr++, value);
				}

				public void ReadValue(ValueRecord rec, int index, double value)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, string value)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, Record value)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, DateTime value)
				{
					throw new InvalidOperationException();
				}
			}
		}
	}
}
