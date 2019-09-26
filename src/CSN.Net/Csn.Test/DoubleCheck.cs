using Csn;
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
				IWriterField fw = w.WriteArray();
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

			class ReadRec : ReadImplExc, IRead, IReadValue
			{
				public override IReadValue GetReadValue()
				{
					return this;
				}

				public override void Read(VersionRecord verRec)
				{
				}

				public override void Read(ArrayRecord arrRec)
				{ }

				int ctr = 0;
				public override void ReadValue(ValueRecord rec, int index, double value)
				{
					Assert.AreEqual(value, ((ctr++) / 100.0));
				}
			}
		}
	}
}
