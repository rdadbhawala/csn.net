using System;
using System.Collections.Generic;
using System.Text;

namespace Csn
{
	public enum ErrorCode
	{
		Unknown,
		UnknownRecordType,
		UnexpectedChars,
		NothingToRead,
		NotEscapeChar,
		UnexpectedEOF,
		UnexpectedRecordType,
	}

	public enum ErrorDataKeys
	{
		Expected,
		Actual,
	}

	public class Error
		: ApplicationException
	{
		public static Error UnexpectedChars(char expected, char actual)
		{
			return Unexpected(ErrorCode.UnexpectedChars, expected, actual);
			//Error err = new Error(ErrorCode.UnexpectedChars);
			//err.Data[ErrorDataKeys.Expected] = expected;
			//err.Data[ErrorDataKeys.Actual] = actual;
			//return err;
		}

		public static Error UnexpectedRecordType(RecordType expected, RecordType actual)
		{
			return Unexpected(ErrorCode.UnexpectedRecordType, expected, actual);
			//Error err = new Error(ErrorCode.UnexpectedRecordType);
			//err.Data[ErrorDataKeys.Expected] = expected;
			//err.Data[ErrorDataKeys.Actual] = actual;
			//return err;
		}

		public static Error Unexpected(ErrorCode pCode, object expected, object actual)
		{
			return new Error(pCode).AddData(ErrorDataKeys.Expected, expected).AddData(ErrorDataKeys.Actual, actual);
		}

		public Error(ErrorCode pCode)
			: base(pCode.ToString())
		{
			this.Code = pCode;
		}

		public ErrorCode Code { get; private set; }

		public Error AddData(ErrorDataKeys key, object value)
		{
			this.Data[key] = value;
			return this;
		}
	}
}
