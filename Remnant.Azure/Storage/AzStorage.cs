using Azure.Core;
using Remnant.Azure.Core;

namespace Remnant.Azure.Storage;
public enum State
{
	Neutral = 0,
	Connected,
	Disconnected,
	Failed
}

public abstract class AzStorage<TClient, TResource, TConfig> : AzResource<TResource, TConfig>
	where TResource : class, IAzure
	where TConfig : class, IAzure
{
	protected TClient? _client;

	public override TResource Connect(TokenCredential? credential = null)
	{
		//Shield
		//	.AgainstNullOrEmpty(_endPoint)
		//	.Raise<EndPointException>(EndPointException.NoEndPointOnConnect);

		_client = (TClient)Activator.CreateInstance(typeof(TClient), _endPoint);
		_actions.ForEach(action => action.method(action.parameters));

		return this as TResource;
	}

	public override TConfig UseEndPoint(Uri endpoint)
	{
		//Shield
		//	.AgainstNull(endpoint)
		//	.Raise();

		_endPoint = endpoint.OriginalString;
		return this as TConfig;
	}

	public override TConfig UseEndPoint(string connectionString)
	{
		//Shield
		//	.AgainstNullOrEmpty(connectionString)
		//	.Raise<EndPointException>(EndPointException.NoEndPoint);

		_endPoint = connectionString;
		return this as TConfig;
	}

}
