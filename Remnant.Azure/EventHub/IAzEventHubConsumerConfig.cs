using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Remnant.Azure.Core;

namespace Remnant.Azure.EventHub;

public interface IAzEventHubConsumerConfig : IAzResource<IAzEventHubProcessor, IAzEventHubProcessorConfig>
{
	IAzEventHubProcessorConfig UseEventHub(string eventHubName);
	IAzEventHubProcessorConfig UseCheckPointsContainer(string checkPointsContainer);
	IAzEventHubProcessorConfig UseCheckPointsEndPoint(string endPoint);
	IAzEventHubProcessorConfig UseConsumerGroup(string consumerGroup);
	IAzEventHubProcessorConfig Configure(EventProcessorClientOptions options);
	IAzEventHubProcessorConfig OnProcessEvent(Func<ProcessEventArgs, Task> processEvent);
	IAzEventHubProcessorConfig OnProcessError(Func<ProcessErrorEventArgs, Task> processError);
	IAzEventHubProcessorConfig AutoCheckPoint(TimeSpan time);
	IAzEventHubProcessorConfig StartFromOffset(long offset);
}