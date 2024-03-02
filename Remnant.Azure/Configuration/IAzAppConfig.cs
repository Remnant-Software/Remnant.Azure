using Remnant.Azure.Core;

namespace Remnant.Azure.Configuration;

public interface IAzAppConfig : IAzure
{
	TAppSetting GetSetting<TAppSetting>(string configurationKey, CancellationToken cancellationToken = default)
		 where TAppSetting : AzAppSetting, new();
}