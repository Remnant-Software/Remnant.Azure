namespace Remnant.Azure.Core;

public interface IAzFactory
{
	TConfig Register<TResource, TConfig>(string name)
		 where TResource : class, IAzure
		 where TConfig : class, IAzure;

	void Register(string name, IAzure resource);
}
