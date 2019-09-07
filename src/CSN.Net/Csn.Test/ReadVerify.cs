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

		internal void Verify(TypeDefRecord typeRec)
		{
			Verify(typeRec.Code);
			Assert.AreEqual(sTypeName.Dequeue(), typeRec.Name);
			CollectionAssert.AreEqual(sTypeMembers.Dequeue(), typeRec.Members);
		}
	}
}
