using Azure.Core;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Remnant.Azure.Core;

namespace Remnant.Azure.EventHub;

public class AzEventHubProcessor :
	AzResource<IAzEventHubProcessor, IAzEventHubProcessorConfig>,
	IAzEventHubProcessor,
	IAzEventHubProcessorConfig
{
	private string _checkPointsEndPoint;
	private string _checkPointsName;
	private string _consumerGroup;
	private string _eventHubName;

	private BlobContainerClient _checkPoints;
	private EventProcessorClient _client;
	private EventProcessorClientOptions _options = new();
	private Func<ProcessEventArgs, Task> _processEvent;
	private Func<ProcessErrorEventArgs, Task> _processError;

	private AzEventHubProcessor()
	{
		_options.PrefetchCount = 10;
		_options.RetryOptions.MaximumRetries = 5;
		_options.MaximumWaitTime = TimeSpan.FromSeconds(15);
	}

	public static IAzEventHubProcessorConfig Create()
	{
		return new AzEventHubProcessor();
	}

	public IAzEventHubProcessorConfig Configure(EventProcessorClientOptions options)
	{
		_options = options;
		return this;
	}

	public override IAzEventHubProcessor Connect(TokenCredential? credential = null)
	{
		_checkPoints = new BlobContainerClient(_checkPointsEndPoint, _checkPointsName);

		_client = new EventProcessorClient(_checkPoints,
			string.IsNullOrEmpty(_consumerGroup)
				? EventHubConsumerClient.DefaultConsumerGroupName
				: _consumerGroup,
			_endPoint,
			_eventHubName,
			_options);

		_client.ProcessEventAsync += _processEvent;
		_client.ProcessErrorAsync += _processError;

		return this;
	}

	public IAzEventHubProcessorConfig UseCheckPointsContainer(string checkPointsContainer)
	{
		_checkPointsName = checkPointsContainer;
		return this;
	}

	public IAzEventHubProcessorConfig UseCheckPointsEndPoint(string endPoint)
	{
		_checkPointsEndPoint = endPoint;
		return this;
	}

	public IAzEventHubProcessorConfig UseConsumerGroup(string consumerGroup)
	{
		_consumerGroup = consumerGroup;
		return this;
	}

	public IAzEventHubProcessorConfig UseEventHub(string eventHubName)
	{
		_eventHubName = eventHubName;
		return this;
	}

	public IAzEventHubProcessorConfig OnProcessEvent(Func<ProcessEventArgs, Task> processEvent)
	{
		_processEvent = processEvent;
		return this;
	}

	public IAzEventHubProcessorConfig OnProcessError(Func<ProcessErrorEventArgs, Task> processError)
	{
		_processError = processError;
		return this;
	}

	public async Task StartAsync(CancellationToken cancellationToken = default)
	{
		await _client.StartProcessingAsync(cancellationToken);
	}

	public async Task StopAsync(CancellationToken cancellationToken = default)
	{
		await _client.StopProcessingAsync(cancellationToken);
	}

	public void Start()
	{
		_client.StartProcessing();
	}

	public void Stop()
	{
		_client.StopProcessing();
	}

	public IAzEventHubProcessorConfig AutoCheckPoint(TimeSpan time)
	{
		return this;
	}
}
