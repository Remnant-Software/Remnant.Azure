using Azure.Core;

namespace Remnant.Azure.Core;

public interface IAzResource<TResource, TConfig> : IAzure
	 where TResource : IAzure
	 where TConfig : IAzure
{
	TConfig UseEndPoint(Uri endpoint);
	TConfig UseEndPoint(string endpoint);
	TResource Connect(TokenCredential? credential = null);
	IAzResource<TResource, TConfig> Resource { get; }
}
