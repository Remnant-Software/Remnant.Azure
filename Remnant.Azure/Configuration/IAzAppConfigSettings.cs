using Remnant.Azure.Core;

namespace Remnant.Azure.Configuration;

public interface IAzAppConfigSettings : IAzResource<IAzAppConfig, IAzAppConfigSettings>
{
	IAzAppConfigSettings UseEnvironment(string environment);
}