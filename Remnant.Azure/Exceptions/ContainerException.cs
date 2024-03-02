namespace Remnant.Azure.Exceptions;

public class ContainerException : AzException
{
	public const string NoContainerName = "Container name not specified.";

	public ContainerException() : base("Container failure.")
	{
	}

	public ContainerException(string? message) : base(message)
	{
	}

	public ContainerException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}
