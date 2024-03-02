using Remnant.Azure.Core;

namespace Remnant.Azure.Storage;

public interface IAzContainerConfig : IAzResource<IAzContainer, IAzContainerConfig>
{
	IAzContainerConfig CreateContainer(string containerName, Dictionary<string, string>? metaData = null, CancellationToken cancellationToken = default);

	IAzContainerConfig DeleteContainer(string containerName, CancellationToken cancellationToken = default);

	IAzContainerConfig UseContainer(string containerName);
}