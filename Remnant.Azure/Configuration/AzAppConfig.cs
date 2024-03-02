using Azure.Core;
using Azure.Data.AppConfiguration;
using Remnant.Azure.Core;

namespace Remnant.Azure.Configuration;

public class AzAppConfig : AzResource<IAzAppConfig, IAzAppConfigSettings>, IAzAppConfig, IAzAppConfigSettings
{
	private ConfigurationClient _client;
	private string _environment;
	private string _configurationKey;

	private AzAppConfig()
	{
	}

	public static IAzAppConfigSettings Create()
	{
		return new AzAppConfig();
	}

	public override IAzAppConfig Connect(TokenCredential? credential = null)
	{
		//Shield
		//	.AgainstNullOrEmpty(_endPoint)
		//	.Raise<EndPointException>(EndPointException.NoEndPointOnConnect);



		_client = new ConfigurationClient(_endPoint);

		return this;
	}

	public IAzAppConfigSettings UseEnvironment(string environment)
	{
		//Shield.
		_environment = environment;
		return this;
	}

	public TAppSetting GetSetting<TAppSetting>(string configurationKey, CancellationToken cancellationToken = default)
		where TAppSetting : AzAppSetting, new()
	{
		//Shield.

		var response = _client.GetConfigurationSetting(configurationKey, _environment, cancellationToken);
		var setting = AzAppSetting.Deserialize<TAppSetting>(response.Value.Value);

		setting.RawSetting = response.Value;

		//todo: get secret cfg from app config linked to vault
		foreach (var resource in setting.azure.resources)
		{
			if (string.IsNullOrEmpty(resource.endPoint))
			{
				response = _client.GetConfigurationSetting(resource.name, _environment, cancellationToken);
				resource.endPoint = response.Value.Value;
			}
		}

		return setting;
	}
}
