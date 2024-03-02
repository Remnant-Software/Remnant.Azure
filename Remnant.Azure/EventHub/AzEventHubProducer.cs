using Azure.Core;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Remnant.Azure.Core;
using Remnant.Azure.Exceptions;

namespace Remnant.Azure.EventHub;

public class AzEventHubProducer :
	AzResource<IAzEventHubProducer, IAzEventHubProducerConfig>,
	IAzEventHubProducer,
	IAzEventHubProducerConfig,
	IDisposable
{
	private string _eventHubName;

	private EventHubProducerClient _client;
	private EventHubProducerClientOptions _options = new();
	private Action<IAzEventHubProducer> _produceEvent;
	private Action<IAzEventHubProducer, Exception> _produceError;
	private TimeSpan _scheduleTime;
	private Timer _timer;
	private bool disposedValue;

	private AzEventHubProducer()
	{
	}

	public static IAzEventHubProducerConfig Create()
	{
		return new AzEventHubProducer();
	}

	public IAzEventHubProducerConfig Configure(EventHubProducerClientOptions options)
	{
		_options = options;
		return this;
	}

	public override IAzEventHubProducer Connect(TokenCredential? credential = null)
	{
		_client = new EventHubProducerClient(_endPoint, _eventHubName, _options);
		return this;
	}

	public IAzEventHubProducerConfig UseEventHub(string eventHubName)
	{
		_eventHubName = eventHubName;
		return this;
	}

	public IAzEventHubProducerConfig OnProduceEvent(Action<IAzEventHubProducer> produceEvent, TimeSpan scheduleTime)
	{
		_scheduleTime = scheduleTime;
		_produceEvent = produceEvent;
		return this;
	}

	public IAzEventHubProducerConfig OnProduceError(Action<IAzEventHubProducer, Exception> producerError)
	{
		_produceError = producerError;
		return this;
	}

	public async Task SendAsync(EventDataBatch data)
	{
		try
		{
			await _client.SendAsync(data);
		}
		catch (Exception e)
		{
			if (_produceError != null)
				_produceError(this, e);
			else
				throw;
		}
	}

	public async Task SendAsync(List<EventData> data)
	{
		try
		{
			await _client.SendAsync(data);
		}
		catch (Exception e)
		{
			if (_produceError != null)
				_produceError(this, e);
			else
				throw;
		}
	}

	public async Task SendAsync(EventData data)
	{
		await SendAsync(new List<EventData> { data });
	}

	public async Task<EventDataBatch> CreateBatchAsync()
	{
		return await _client.CreateBatchAsync();
	}

	protected virtual async void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_timer?.Dispose();
				await _client.DisposeAsync();
			}

			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public async Task StartAsync()
	{
		if (_produceEvent == null)
			throw new AzException("Cannot start eventhub producer if scheduled time and produce event callback not been specified. See member 'OnProduceEvent'.");

		_timer = new Timer((callback) =>
		{
			_produceEvent(this);
		}, null, 0, _scheduleTime.Milliseconds);
	}

	public void Stop()
	{
		_timer.DisposeAsync().GetAwaiter().GetResult();
	}
}
