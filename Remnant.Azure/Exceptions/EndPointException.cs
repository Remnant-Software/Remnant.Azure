namespace Remnant.Azure.Exceptions;

public class EndPointException : AzException
{
	public const string NoEndPointOnConnect = "End point/connection must be specified before calling connect.";
	public const string NoEndPoint = "End point/connection not specified.";

	public EndPointException() : base("End point failure.")
	{
	}


	public EndPointException(string? message) : base(message)
	{
	}

	public EndPointException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}
