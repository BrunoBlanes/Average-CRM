using System;

namespace CRM.Shared.Exceptions
{
	public class WebApiException : Exception
	{
		public WebApiException(string message) : base(message) { }
		public WebApiException(string message, Exception innerException) : base(message, innerException) { }
		public WebApiException() { }
	}
}