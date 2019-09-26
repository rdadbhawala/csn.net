using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csn.Test
{
	class ReadSetup
	{
		protected Queue<RecordCode> sRecCodes = new Queue<RecordCode>();
		protected Queue<string> sTypeName = new Queue<string>();
		protected Queue<string[]> sTypeMembers = new Queue<string[]>();
		protected Queue<long> sRefSeqNos = new Queue<long>();
		protected Queue<bool> sValuesBool = new Queue<bool>();
		protected Queue<DateTime> sValuesDt = new Queue<DateTime>();
		protected Queue<double> sValuesDouble = new Queue<double>();
		protected Queue<long> sValuesLong = new Queue<long>();
		protected Queue<string> sValuesStr = new Queue<string>();
		protected Queue<PrimitiveType> sPrimTypes = new Queue<PrimitiveType>();
		protected Queue<object> sValuesNull = new Queue<object>();

		public void SetupVersionRecord(string version)
		{
			sRecCodes.Enqueue(new RecordCode(RecordType.Version, 0));
			sValuesStr.Enqueue(version);
		}

		public void SetupTypeRecord(RecordCode rc, string name, params string[] members)
		{
			sRecCodes.Enqueue(rc);
			sTypeName.Enqueue(name);
			sTypeMembers.Enqueue(members);
		}

		public ReadSetup SetupArray(RecordCode rc)
		{
			sRecCodes.Enqueue(rc);
			return this;
		}

		public ReadSetup SetupArrayPrims(RecordCode rc, PrimitiveType p)
		{
			sRecCodes.Enqueue(rc);
			sPrimTypes.Enqueue(p);
			return this;
		}

		public ReadSetup SetupInstance(RecordCode recordCode, long refType)
		{
			sRecCodes.Enqueue(recordCode);
			sRefSeqNos.Enqueue(refType);
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

		public ReadSetup SetupRefs(long refSeq)
		{
			sRefSeqNos.Enqueue(refSeq);
			return this;
		}

		public ReadSetup SetupNull()
		{
			sValuesNull.Enqueue(null);
			return this;
		}
	}

	class ReadVerify : ReadSetup
	{
		protected void Verify(RecordType pType, long pSequenceNo)
		{
			RecordCode expected = sRecCodes.Dequeue();
			Assert.AreEqual(expected.RecType, pType);
			Assert.AreEqual(expected.SequenceNo, pSequenceNo);
		}

		[Obsolete]
		protected RecordCode Verify(RecordCode actual)
		{
			RecordCode expected = sRecCodes.Dequeue();
			Assert.AreEqual(expected.RecType, actual.RecType);
			Assert.AreEqual(expected.SequenceNo, actual.SequenceNo);
			return expected;
		}

		public void VerifyVersionRecord(VersionRecord verRec)
		{
			Verify(RecordType.Version, verRec.SequenceNo);
			Assert.AreEqual(sValuesStr.Dequeue(), verRec.Version);
		}

		public void VerifyTypeDefWithMembers(TypeDefRecord typeRec)
		{
			Verify(RecordType.TypeDef, typeRec.SequenceNo);
			Assert.AreEqual(sTypeName.Dequeue(), typeRec.Name);
			CollectionAssert.AreEqual(sTypeMembers.Dequeue(), typeRec.Members);
		}

		public void Verify(InstanceRecord instRec)
		{
			Verify(RecordType.Instance, instRec.SequenceNo);
			Assert.AreEqual(sRefSeqNos.Dequeue(), instRec.Ref.SequenceNo);
		}

		public void VerifyArray(ArrayRecord arrRec)
		{
			Verify(RecordType.Array, arrRec.SequenceNo);
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

		internal void Verify(DateTime value)
		{
			Assert.AreEqual(sValuesDt.Dequeue(), value);
		}

		internal void VerifyRefs(long refSeqNo)
		{
			Assert.AreEqual(sRefSeqNos.Dequeue(), refSeqNo);
		}

		internal void VerifyNull()
		{
			Assert.IsNull(sValuesNull.Dequeue());
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
			this.VerifyVersionRecord(verRec);
		}

		public void Read(TypeDefRecord typeRec)
		{
			this.VerifyTypeDefWithMembers(typeRec);
		}

		public void Read(InstanceRecord instRec)
		{
			this.Verify(instRec);
		}

		public void Read(ArrayRecord arrRec)
		{
			this.VerifyArray(arrRec);
		}

		public void ReadValueNull(ValueRecord rec, int index)
		{
			this.VerifyNull();
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
			this.VerifyRefs(obj.SequenceNo);
		}

		public void ReadValue(ValueRecord rec, int index, DateTime value)
		{
			this.Verify(value);
		}
	}
}
