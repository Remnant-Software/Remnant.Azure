using Azure.Data.AppConfiguration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Remnant.Azure.Configuration;
using Remnant.Azure.Core;

namespace Remnant.Azure.Tests
{
    public class TestAppConfig
	{
		private readonly string _endPoint;
		private readonly string _configurationKey;
		private IAzResource<IAzAppConfig, IAzAppConfigSettings> _appConfigResource;

		public TestAppConfig()
		{
			_configurationKey = "Compare.Tool";
			_endPoint = @"";
		}

		[SetUp]
		public void Setup()
		{
			_appConfigResource = AzAppConfig
				.Create()
				.UseEndPoint(_endPoint)
				.UseEnvironment("dev")
				.Resource;
		}

		[Test]
		public void Must_be_able_to_connect()
		{
			Assert.That(_appConfigResource.Connect(), Is.Not.Null);
		}

		[Test]
		public void Must_be_able_to_get_configuration_setting()
		{
			var config = _appConfigResource
				.Connect()
				.GetSetting<AzAppSetting>(_configurationKey);
		}

	}
}
