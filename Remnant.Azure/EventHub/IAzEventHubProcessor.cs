using Remnant.Azure.Core;

namespace Remnant.Azure.EventHub;

public interface IAzEventHubProcessor : IAzure
{
	Task StartAsync(CancellationToken cancellationToken = default);
	Task StopAsync(CancellationToken cancellationToken = default);

	void Start();
	void Stop();
}