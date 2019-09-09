using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstraction.Csn.Test
{
	class ReadSetup
	{
		protected Queue<RecordCode> sRecCodes = new Queue<RecordCode>();
		protected Queue<string> sTypeName = new Queue<string>();
		protected Queue<string[]> sTypeMembers = new Queue<string[]>();
		protected Queue<int> sInstanceTypeRef = new Queue<int>();
		protected Queue<bool> sValuesBool = new Queue<bool>();
		protected Queue<DateTime> sValuesDt = new Queue<DateTime>();
		protected Queue<double> sValuesDouble = new Queue<double>();
		protected Queue<long> sValuesLong = new Queue<long>();
		protected Queue<string> sValuesStr = new Queue<string>();

		public void SetupVersionRecord()
		{
			sRecCodes.Enqueue(new RecordCode(RecordType.Version, 0));
		}

		public void Setup(RecordCode rc, string name, params string[] members)
		{
			sRecCodes.Enqueue(rc);
			sTypeName.Enqueue(name);
			sTypeMembers.Enqueue(members);
		}

		public ReadSetup Setup(RecordCode recordCode, int refType)
		{
			sRecCodes.Enqueue(recordCode);
			sInstanceTypeRef.Enqueue(refType);
			return this;
		}

		public ReadSetup Setup(bool v)
		{
			sValuesBool.Enqueue(v);
			return this;
		}

		public ReadSetup Setup(DateTime dateTime)
		{
			sValuesDt.Enqueue(dateTime);
			return this;
		}

		public ReadSetup Setup(double value)
		{
			sValuesDouble.Enqueue(value);
			return this;
		}

		public ReadSetup Setup(string str)
		{
			sValuesStr.Enqueue(str);
			return this;
		}

		public ReadSetup Setup(long value)
		{
			sValuesLong.Enqueue(value);
			return this;
		}
	}

	class ReadVerify : ReadSetup
	{
		protected void Verify(RecordCode actual)
		{
			RecordCode expected = sRecCodes.Dequeue();
			Assert.AreEqual(expected.RecType, actual.RecType);
			Assert.AreEqual(expected.SequenceNo, actual.SequenceNo);
		}

		public void Verify(VersionRecord verRec)
		{
			Verify(verRec.Code);
		}

		public void Verify(TypeDefRecord typeRec)
		{
			Verify(typeRec.Code);
			Assert.AreEqual(sTypeName.Dequeue(), typeRec.Name);
			CollectionAssert.AreEqual(sTypeMembers.Dequeue(), typeRec.Members);
		}

		public void Verify(ArrayRecord arrRec)
		{
			Verify(arrRec.Code);
		}

		public void Verify(long value)
		{
			Assert.AreEqual(sValuesLong.Dequeue(), value);
		}

		internal void Verify(double value)
		{
			Assert.AreEqual(sValuesDouble.Dequeue(), value);
		}

		public void Verify(bool value)
		{
			Assert.AreEqual(sValuesBool.Dequeue(), value);
		}

		internal void Verify(string value)
		{
			Assert.AreEqual(sValuesStr.Dequeue(), value);
		}

		public void Verify(InstanceRecord instRec)
		{
			Verify(instRec.Code);
			Assert.AreEqual(sInstanceTypeRef.Dequeue(), instRec.Ref.Code.SequenceNo);
		}

		internal void Verify(DateTime value)
		{
			Assert.AreEqual(sValuesDt.Dequeue(), value);
		}
	}

	class ReadTest : ReadVerify, IRead, IReadValue
	{
		public IReadValue GetReadValue()
		{
			return this;
		}

		public void Read(VersionRecord verRec)
		{
			this.Verify(verRec);
		}

		public void Read(TypeDefRecord typeRec)
		{
			this.Verify(typeRec);
		}

		public void Read(InstanceRecord instRec)
		{
			this.Verify(instRec);
		}

		public void Read(ArrayRecord arrRec)
		{
			this.Verify(arrRec);
		}

		public void ReadValue(ValueRecord rec, int index, PrimitiveNull value)
		{
			Console.WriteLine(index + ") Value null");
		}

		public void ReadValue(ValueRecord rec, int index, bool value)
		{
			this.Verify(value);
		}

		public void ReadValue(ValueRecord rec, int index, long value)
		{
			this.Verify(value);
		}

		public void ReadValue(ValueRecord rec, int index, double value)
		{
			this.Verify(value);
		}

		public void ReadValue(ValueRecord rec, int index, string value)
		{
			this.Verify(value);
		}

		public void ReadValue(ValueRecord rec, int index, Record obj)
		{
			//Console.WriteLine(index + ") Value " + obj.Code.SequenceNo);
		}

		public void ReadValue(ValueRecord rec, int index, DateTime value)
		{
			this.Verify(value);
		}
	}
}
