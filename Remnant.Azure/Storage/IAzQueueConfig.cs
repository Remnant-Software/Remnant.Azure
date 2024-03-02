using Azure.Storage.Queues;
using Remnant.Azure.Core;

namespace Remnant.Azure.Storage;

public interface IAzQueueConfig : IAzResource<IAzQueue, IAzQueueConfig>
{
	IAzQueueConfig CreateQueue(string queueName, Dictionary<string, string>? metaData = null, CancellationToken cancellationToken = default);

	IAzQueueConfig DeleteQueue(string queueName, CancellationToken cancellationToken = default);

	IAzQueueConfig UseQueue(string queueName);

	IAzQueueConfig UseEncoding(QueueMessageEncoding messageEncoding);

}