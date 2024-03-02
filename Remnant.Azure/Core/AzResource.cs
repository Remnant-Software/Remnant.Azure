using Azure.Core;

namespace Remnant.Azure.Core;

public abstract class AzResource<TResource, TConfig> : IAzResource<TResource, TConfig>
	where TResource : class, IAzure
	where TConfig : class, IAzure
{
	protected string? _endPoint;
	protected TokenCredential? _credential;
	protected List<(Func<object[], object> method, object[] parameters)> _actions = new List<(Func<object[], object> method, object[] parameters)>();

	public AzResource() { }

	public IAzResource<TResource, TConfig> Resource => this;

	public abstract TResource Connect(TokenCredential? credential = null);

	public virtual TConfig UseEndPoint(Uri endpoint)
	{
		//Shield
		//	.AgainstNull(endpoint)
		//	.Raise();

		_endPoint = endpoint.OriginalString;
		return this as TConfig;
	}

	public virtual TConfig UseEndPoint(string endpoint)
	{
		//Shield
		//	.AgainstNull(endpoint)
		//	.Raise();

		_endPoint = endpoint;
		return this as TConfig;
	}
}
