using System;
using System.Collections.Generic;
using System.Text;

namespace Abstraction.Csn
{
	class ReaderVersionSelector : ReadStateBase
	{
		public static readonly ReaderVersionSelector Singleton = new ReaderVersionSelector();
		private ReaderVersionSelector() { }

		public override void Read(ReadArgs args)
		{
			// read Version Record Code 'V0,'
			if (args.Stream.Read() != ReaderHelper.iVersion || args.Stream.Read() != ReaderHelper.iDigit0 || args.Stream.Read() != ReaderHelper.iFieldSep)
			{
				throw Error.UnexpectedRecordType(RecordType.Version, RecordType.Unknown);
			}
			args.CurrentRC = new RecordCode(RecordType.Version, 0);

			// TODO - version validation ??

			VersionRecord vr = new VersionRecord(args.CurrentRC, base.ReadStringStrict(args));
			args.dcRecords[vr.Code.SequenceNo] = vr;
			args.Read.Read(vr);

			base.ReadExpectNewRecord(args, args.Stream.Read());
		}
	}
}
