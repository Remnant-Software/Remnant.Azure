using Azure.Messaging.EventHubs;
using Remnant.Azure.Core;
using Remnant.Azure.EventHub;
using Remnant.Azure.Exceptions;
using System.Text;

namespace Remnant.Azure.Tests;

public class TestEventHubProducer
{
	private readonly string _eventHubEndPoint;
	private readonly string _eventHubName;
	private IAzResource<IAzEventHubProducer, IAzEventHubProducerConfig> _eventHubResource;

	public TestEventHubProducer()
	{
		_eventHubEndPoint = @"";
		_eventHubName = "";
	}

	[SetUp]
	public void Setup()
	{

		_eventHubResource = AzEventHubProducer
			.Create()
			.UseEndPoint(_eventHubEndPoint)
			.UseEventHub(_eventHubName)
			.OnProduceEvent(OnProduceEventHandler, TimeSpan.FromSeconds(2))
			.OnProduceError(OnProduceErrorHandler);
		//.Configure(options)
	}

	private void OnProduceEventHandler(IAzEventHubProducer producer)
	{
		using var eventBatch = producer.CreateBatchAsync().Result;
		eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Test")));
		producer.SendAsync(eventBatch).Wait();
	}

	private void OnProduceErrorHandler(IAzEventHubProducer producer, Exception e)
	{
		Assert.That(e, Is.Not.Null);
	}

	[Test]
	public void Must_be_able_to_connect()
	{
		using var eventHub = _eventHubResource.Connect();
		Assert.That(eventHub, Is.Not.Null);
	}

	[Test]
	public void Must_be_able_to_user_schedule_timer()
	{
		using var eventHub = _eventHubResource.Connect();
		eventHub.StartAsync();

		Task.Delay(TimeSpan.FromSeconds(10)).Wait();

		eventHub.Stop();
	}

	[Test]
	public void Must_be_able_to_use_event()
	{
		using var eventHub = _eventHubResource.Connect();
		using var eventBatch = eventHub.CreateBatchAsync().Result;

		eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Test")));
		eventHub.SendAsync(eventBatch).Wait();
	}
}
