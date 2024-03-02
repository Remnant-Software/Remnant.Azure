namespace Remnant.Azure.Exceptions
{
	public class AzException : Exception
	{
		public AzException(string? message) : base(message)
		{
		}

		public AzException(string? message, Exception? innerException) : base(message, innerException)
		{
		}
	}
}
