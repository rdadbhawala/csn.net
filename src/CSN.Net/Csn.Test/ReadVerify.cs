using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstraction.Csn.Test
{
	class ReadVerify
	{
		Queue<RecordCode> sRecCodes = new Queue<RecordCode>();
		Queue<string> sTypeName = new Queue<string>();
		Queue<string[]> sTypeMembers = new Queue<string[]>();
		Queue<int> sInstanceTypeRef = new Queue<int>();
		Queue<bool> sValuesBool = new Queue<bool>();

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

		public ReadVerify Setup(RecordCode recordCode, int refType)
		{
			sRecCodes.Enqueue(recordCode);
			sInstanceTypeRef.Enqueue(refType);
			return this;
		}

		public ReadVerify Setup(bool v)
		{
			sValuesBool.Enqueue(v);
			return this;
		}

		private void Verify(RecordCode actual)
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

		public void Verify(bool value)
		{
			Assert.AreEqual(sValuesBool.Dequeue(), value);
		}

		public void Verify(InstanceRecord instRec)
		{
			Verify(instRec.Code);
			Assert.AreEqual(sInstanceTypeRef.Dequeue(), instRec.Ref.Code.SequenceNo);
		}
	}
}
