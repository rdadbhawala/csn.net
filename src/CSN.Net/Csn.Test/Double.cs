using Abstraction.Csn;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace Performance
{
	[TestClass]
	public class DoubleCheck
	{
		public static long ctr = 10000;
		public static double[] values = null;

		static DoubleCheck()
		{
			values = new double[ctr];
			for (int i = 0; i < ctr; i++)
			{
				values[i] = i / 100.0;
			}
		}

		[TestMethod]
		public void Program()
		{
			//DoubleSer ser = new DoubleSer();
			//ser.Setup();
			//ser.DoubleSerDo();
			//ser.tgt.Position = 0;
			//Console.WriteLine(new StreamReader(ser.tgt, Encoding.UTF8).ReadToEnd());
			DoubleDeser deser = new DoubleDeser();
			deser.Setup();
			deser.DoubleDeserDo();
		}

		private static Stream GetStream()
		{
			return new MemoryStream();
		}

		public class DoubleSer
		{
			StreamWriter sw = null;
			Writer w = null;
			internal Stream tgt = DoubleCheck.GetStream();

			public void Setup()
			{
				sw = new StreamWriter(tgt, Encoding.UTF8);
				w = new Writer(sw);
			}

			public void DoubleSerDo()
			{
				IWriterField fw = w.WriteArray(PrimitiveType.Int);
				for (long i = 0; i < DoubleCheck.ctr; i++)
				{
					fw.W(values[i]);
				}
				sw.Flush();
			}
		}

		public class DoubleDeser
		{
			Stream mStream = new MemoryStream();
			StreamReader sr = null;
			ReadRec rr = null;

			public DoubleDeser()
			{
				DoubleSer ser = new DoubleSer
				{
					tgt = mStream
				};
				ser.Setup();
				ser.DoubleSerDo();
			}

			public void Setup()
			{
				mStream.Position = 0;
				sr = new StreamReader(mStream, Encoding.UTF8);
				rr = new ReadRec();
			}

			public void DoubleDeserDo()
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

				public void Read(ArrayRefsRecord arrayRec)
				{
					throw new InvalidOperationException();
				}

				public void Read(ArrayPrimitivesRecord arrRec)
				{
				}

				public void ReadValueNull(ValueRecord rec, int index)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, bool value)
				{
					throw new InvalidOperationException();
				}

				public void ReadValue(ValueRecord rec, int index, long value)
				{
					throw new InvalidOperationException();
				}

				int ctr = 0;
				public void ReadValue(ValueRecord rec, int index, double value)
				{
					Assert.AreEqual(value, ((ctr++) / 100.0));
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
