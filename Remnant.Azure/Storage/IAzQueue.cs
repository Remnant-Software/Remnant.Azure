using Azure;
using Azure.Storage.Queues.Models;

namespace Remnant.Azure.Storage;

public interface IAzQueue : IAzStorage
{
	/// <summary>
	/// Delete all messages from the queue
	/// </summary>
	/// <returns>Http response</returns>
	Response DeleteAllMessages();

	/// <summary>
	/// Delete message from the queue
	/// </summary>
	/// <param name="messageId">The Id of the message to delete</param>
	/// <param name="popReceipt">The pop receipt of the message</param>
	/// <param name="cancellationToken">Optional cancellation token</param>	
	/// <returns>Http response</returns>
	Response DeleteMessage(string messageId, string popReceipt, CancellationToken cancellationToken = default);

	/// <summary>
	/// Update message on the queue (re-enqueue)
	/// </summary>
	/// <param name="messageId">The Id of the message to update</param>
	/// <param name="popReceipt">The pop receipt of the message</param>
	/// <param name="content">The content to use for the update</param>
	/// <param name="cancellationToken">Optional cancellation token</param>
	/// <returns></returns>
	(UpdateReceipt, Response<UpdateReceipt>) UpdateMessage(string messageId, string popReceipt, string content, CancellationToken cancellationToken = default);

	/// <summary>
	/// Send message to the queue (enqueue)
	/// </summary>
	/// <param name="message">Mssage to be sent</param>
	/// <returns>Send receipt, Http response</returns>
	(SendReceipt, Response<SendReceipt>) SendMessage(string message);

	/// <summary>
	/// Read message from the queue (dequeue)
	/// </summary>
	/// <param name="cancellationToken">Optional cancellation token</param>
	/// <returns>Queued message, Http response</returns>
	(QueueMessage, Response<QueueMessage>) ReadMessage(CancellationToken cancellationToken = default);

	/// <summary>
	/// Peek messages from the queue without deqeueing
	/// </summary>
	/// <param name="maxMessages">Total messages to peek (default = 1)</param>
	/// <param name="cancellationToken">Optional cancellation token</param>
	/// <returns>List of peek messages, Http response</returns>
	(IList<PeekedMessage>, Response<PeekedMessage[]>) PeekMessages(int maxMessages = 1, CancellationToken cancellationToken = default);

	/// <summary>
	/// Read messages from the queue (dequeue)
	/// </summary>
	/// <param name="maxMessages">Total messages to read (default = 1)</param>
	/// <param name="cancellationToken">Optional cancellation token</param>
	/// <returns>List of read messages, Http response</returns>
	(List<QueueMessage>, Response<QueueMessage[]>) ReadMessages(int maxMessages = 1, CancellationToken cancellationToken = default);
}