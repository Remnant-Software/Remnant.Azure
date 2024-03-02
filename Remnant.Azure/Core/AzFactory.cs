namespace Remnant.Azure.Core;

public class AzFactory : IAzFactory
{
	private readonly Dictionary<string, object> _registery = new();

	public TConfig Register<TResource, TConfig>(string name)
		where TResource : class, IAzure
		where TConfig : class, IAzure
	{
		var resource = Activator.CreateInstance(typeof(TResource));

		_registery.Add(name, resource);

		return resource as TConfig;
	}

	public TObject Get<TObject>(string name)
		where TObject : class, IAzure
	{
		var resource = _registery[name];
		return resource as TObject;
	}

	public void Register(string name, IAzure resource)
	{
		_registery[name] = resource;
	}
}
