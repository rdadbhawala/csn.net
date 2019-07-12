using System;
using System.Collections.Generic;
using System.Text;

namespace Abstraction.Csn
{
	public enum ErrorCode
	{
		Unknown,
		UnknownRecordType,
		UnexpectedChars,
		NothingToRead,
	}

	public enum ErrorDataKeys
	{
		ExpectedChar,
		ActualChar,
	}

    public class Error
		: ApplicationException
    {
		public static Error UnexpectedChars(char expected, char actual)
		{
			Error err = new Error(ErrorCode.UnexpectedChars);
			err.Data[ErrorDataKeys.ExpectedChar] = expected;
			err.Data[ErrorDataKeys.ActualChar] = actual;
			return err;
		}

		public Error(ErrorCode pCode)
		{
			this.Code = pCode;
		}

		public ErrorCode Code { get; private set; }
    }
}
