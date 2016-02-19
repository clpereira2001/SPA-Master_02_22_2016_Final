using System;
using System.Runtime.Serialization;

namespace Vauction.Exceptions
{
	public class ValidationException : ApplicationException
	{
		public ValidationException()
			: base()
		{
		}

		public ValidationException(string message)
			: base(message)
		{
		}

		public ValidationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public ValidationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
