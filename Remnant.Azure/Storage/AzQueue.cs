using Azure;
using Azure.Core;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace Remnant.Azure.Storage;

public class AzQueue : AzStorage<QueueServiceClient, IAzQueue, IAzQueueConfig>, IAzQueue, IAzQueueConfig
{
	private QueueClient _queue;
	private readonly QueueClientOptions _options = new();
	private string? _queueName;

	private AzQueue()
	{
		_options.MessageEncoding = QueueMessageEncoding.Base64;
	}

	public static IAzQueueConfig Create()
	{
		return new AzQueue();
	}

	public override IAzQueue Connect(TokenCredential? credential = null)
	{
		base.Connect(credential);

		if (!string.IsNullOrEmpty(_queueName))
			_queue = new QueueClient(_endPoint, _queueName, _options);

		return this;
	}

	public Response DeleteAllMessages()
	{
		return _queue.ClearMessages();
	}

	public Response DeleteMessage(string messageId, string popReceipt, CancellationToken cancellationToken = default)
	{
		return _queue.DeleteMessage(messageId, popReceipt, cancellationToken: cancellationToken);
	}

	public (IList<PeekedMessage>, Response<PeekedMessage[]>) PeekMessages(int maxMessages = 1, CancellationToken cancellationToken = default)
	{
		Response<PeekedMessage[]> response = _queue.PeekMessages(maxMessages, cancellationToken);
		return (response.Value.ToList(), response);
	}

	public (List<QueueMessage>, Response<QueueMessage[]>) ReadMessages(int maxMessages = 1, CancellationToken cancellationToken = default)
	{
		Response<QueueMessage[]> response = _queue.ReceiveMessages(maxMessages, cancellationToken: cancellationToken);
		return (response.Value.ToList(), response);
	}

	public (SendReceipt, Response<SendReceipt>) SendMessage(string message)
	{
		var response = _queue.SendMessage(message);
		return (response.Value, response);
	}

	public (QueueMessage, Response<QueueMessage>) ReadMessage(CancellationToken cancellationToken = default)
	{
		var response = _queue.ReceiveMessage(cancellationToken: cancellationToken);
		return (response.Value, response);
	}

	public (UpdateReceipt, Response<UpdateReceipt>) UpdateMessage(string messageId, string popReceipt, string content, CancellationToken cancellationToken = default)
	{
		var response = _queue.UpdateMessage(messageId, popReceipt, content, cancellationToken: cancellationToken);
		return (response.Value, response);
	}

	#region Configuration

	public IAzQueueConfig UseEncoding(QueueMessageEncoding messageEncoding)
	{
		_options.MessageEncoding = messageEncoding;
		return this;
	}

	public IAzQueueConfig UseQueue(string queueName)
	{
		//Shield
		//	.AgainstNullOrEmpty(queueName)
		//	.Raise<QueueException>(QueueException.NoQueueName);

		_queueName = queueName;
		return this;
	}

	public IAzQueueConfig CreateQueue(string queueName, Dictionary<string, string>? metaData = null, CancellationToken cancellationToken = default)
	{
		//Shield
		//	.AgainstNullOrEmpty(queueName)
		//	.Raise<QueueException>(QueueException.NoQueueName);

		object createQueue(object[] args)
		{
			var response = _client.CreateQueue(args[0].ToString(), (Dictionary<string, string>)args[1], (CancellationToken)args[2]);
			_queue = response.Value;
			return this;
		}

		_actions.Add((createQueue, new object[] { queueName, metaData, cancellationToken }));
		return this;
	}

	public IAzQueueConfig DeleteQueue(string queueName, CancellationToken cancellationToken = default)
	{
		//Shield
		//	.AgainstNullOrEmpty(queueName)
		//	.Raise<QueueException>(QueueException.NoQueueName);

		object deleteQueue(object[] args)
		{
			var response = _client.DeleteQueue(args[0].ToString(), (CancellationToken)args[1]);

			if (!string.IsNullOrEmpty(_queueName) && _queueName.Equals(queueName))
				_queueName = null;

			return this;
		}

		_actions.Add((deleteQueue, new object[] { queueName, cancellationToken }));
		return this;
	}

	#endregion
}

