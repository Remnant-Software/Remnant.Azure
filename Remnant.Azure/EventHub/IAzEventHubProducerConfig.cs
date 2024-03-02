using Azure.Messaging.EventHubs.Producer;
using Remnant.Azure.Core;

namespace Remnant.Azure.EventHub;

public interface IAzEventHubProducerConfig : IAzResource<IAzEventHubProducer, IAzEventHubProducerConfig>
{
	IAzEventHubProducerConfig UseEventHub(string eventHubName);
	IAzEventHubProducerConfig Configure(EventHubProducerClientOptions options);
	IAzEventHubProducerConfig OnProduceEvent(Action<IAzEventHubProducer> producerEvent, TimeSpan scheduleTime);
	IAzEventHubProducerConfig OnProduceError(Action<IAzEventHubProducer, Exception> producerError);
}