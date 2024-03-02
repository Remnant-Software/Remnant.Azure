namespace Remnant.Azure.Exceptions;

public class QueueException : AzException
{
	public const string NoQueueName = "Queue name not specified.";

	public QueueException() : base("Queue failure.")
	{
	}

	public QueueException(string? message) : base(message)
	{
	}

	public QueueException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}
