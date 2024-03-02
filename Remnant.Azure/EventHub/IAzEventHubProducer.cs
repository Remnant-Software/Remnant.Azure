using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Remnant.Azure.Core;

namespace Remnant.Azure.EventHub;

public interface IAzEventHubProducer : IAzure, IDisposable
{
	Task SendAsync(EventDataBatch batch);
	Task SendAsync(List<EventData> data);
	Task SendAsync(EventData data);
	Task<EventDataBatch> CreateBatchAsync();

	Task StartAsync();
	void Stop();
}