using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Remnant.Azure.Core;
using Remnant.Azure.EventHub;
using System.Text;

namespace Remnant.Azure.Tests;

public class TestEventHubProcessor
{
	private readonly string _checkPointsEndPoint;
	private readonly string _checkPointsContainerName;
	private readonly string _eventHubEndPoint;
	private readonly string _eventHubName;
	private IAzResource<IAzEventHubProcessor, IAzEventHubProcessorConfig> _eventHubResource;

	public TestEventHubProcessor()
	{

		_checkPointsEndPoint = @"";
		_checkPointsContainerName = "dev-neill-checkpoints";
		_eventHubEndPoint = @"";
		_eventHubName = "";
	}

	[SetUp]
	public void Setup()
	{
		var options = new EventProcessorClientOptions
		{
			MaximumWaitTime = TimeSpan.FromDays(60),
			PrefetchCount = 100
		};

		_eventHubResource = AzEventHubProcessor
			.Create()
			.UseEndPoint(_eventHubEndPoint)
			.UseEventHub(_eventHubName)
			.UseCheckPointsEndPoint(_checkPointsEndPoint)
			.UseCheckPointsContainer(_checkPointsContainerName)
			.OnProcessEvent(OnProcessEventHandler)
			.OnProcessError(OnProcessErrorHandler)
			.Configure(options)
			.UseConsumerGroup("useventhubreader");
	}

	Task OnProcessEventHandler(ProcessEventArgs eventArgs)
	{
		// Write the body of the event to the console window
		Console.WriteLine("\tReceived event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
		eventArgs.UpdateCheckpointAsync().Wait();
		return Task.CompletedTask;
	}

	Task OnProcessErrorHandler(ProcessErrorEventArgs eventArgs)
	{
		// Write details about the error to the console window
		Console.WriteLine($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
		Console.WriteLine(eventArgs.Exception.Message);
		return Task.CompletedTask;
	}

	[Test]
	public void Must_be_able_to_connect()
	{
		Assert.That(_eventHubResource.Connect(), Is.Not.Null);
	}

	[Test]
	public void Must_be_able_to_start()
	{
		var eventHub = _eventHubResource.Connect();

		eventHub.Start();

		Thread.Sleep(100000);
	}
}
