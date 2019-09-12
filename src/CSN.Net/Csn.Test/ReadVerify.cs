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
		protected Queue<long> sRefSeqNos = new Queue<long>();
		protected Queue<bool> sValuesBool = new Queue<bool>();
		protected Queue<DateTime> sValuesDt = new Queue<DateTime>();
		protected Queue<double> sValuesDouble = new Queue<double>();
		protected Queue<long> sValuesLong = new Queue<long>();
		protected Queue<string> sValuesStr = new Queue<string>();
		protected Queue<PrimitiveType> sPrimTypes = new Queue<PrimitiveType>();

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

		public ReadSetup SetupArrayPrims(RecordCode rc, PrimitiveType p)
		{
			sRecCodes.Enqueue(rc);
			sPrimTypes.Enqueue(p);
			return this;
		}

		public ReadSetup SetupInstanceOrArrayRefs(RecordCode recordCode, long refType)
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
	}

	class ReadVerify : ReadSetup
	{
		protected RecordCode Verify(RecordCode actual)
		{
			RecordCode expected = sRecCodes.Dequeue();
			Assert.AreEqual(expected.RecType, actual.RecType);
			Assert.AreEqual(expected.SequenceNo, actual.SequenceNo);
			return expected;
		}

		public void VerifyVersionRecord(VersionRecord verRec)
		{
			Verify(verRec.Code);
			Assert.AreEqual(sValuesStr.Dequeue(), verRec.Version);
		}

		public void VerifyTypeDefWithMembers(TypeDefRecord typeRec)
		{
			Verify(typeRec.Code);
			Assert.AreEqual(sTypeName.Dequeue(), typeRec.Name);
			CollectionAssert.AreEqual(sTypeMembers.Dequeue(), typeRec.Members);
		}

		public void Verify(InstanceRecord instRec)
		{
			Verify(instRec.Code);
			Assert.AreEqual(sRefSeqNos.Dequeue(), instRec.Ref.Code.SequenceNo);
		}

		public void VerifyArrayRefs(ArrayRefsRecord arrRec)
		{
			Verify(arrRec.Code);
			Assert.AreEqual(sRefSeqNos.Dequeue(), arrRec.TypeRef.Code.SequenceNo);
		}

		public void VerifyArrayPrims(ArrayPrimitivesRecod arrRec)
		{
			Verify(arrRec.Code);
			Assert.AreEqual(sPrimTypes.Dequeue(), arrRec.Primitive);
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

		public void Read(ArrayRefsRecord arrRec)
		{
			this.VerifyArrayRefs(arrRec);
		}

		public void Read(ArrayPrimitivesRecod arrRec)
		{
			this.VerifyArrayPrims(arrRec);
		}

		public void ReadValueNull(ValueRecord rec, int index)
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
			this.VerifyRefs(obj.Code.SequenceNo);
		}

		public void ReadValue(ValueRecord rec, int index, DateTime value)
		{
			this.Verify(value);
		}
	}
}
