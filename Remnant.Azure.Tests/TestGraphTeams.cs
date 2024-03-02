using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Remnant.Azure.Core;
using Remnant.Azure.Graph;
using System.Net.Http.Headers;

namespace Remnant.Azure.Tests;

public class TestGraphTeams
{
	//private readonly string _teamId; // prod channel
	private readonly string _newOrleansChatId = "";
	private readonly string _endPoint;
	private IAzResource<IAzGraph, IAzGraph> _graphResource;

	public TestGraphTeams()
	{
		_endPoint = @"";
	}

	[SetUp]
	public void Setup()
	{
		_graphResource = AzGraph
			.Create()
			.UseEndPoint(_endPoint)
			.Resource;
	}

	[Test]
	public void Must_be_able_to_connect()
	{

		

		//return TokenProvider.CreateAzureActiveDirectoryTokenProvider(
		//  async (audience, authority, state) =>
		//  {
		//	  var defaultAzureCredential = new DefaultAzureCredential();
		//	  var trc = new TokenRequestContext(new[] { authority });
		//	  return (await defaultAzureCredential.GetTokenAsync(trc)).Token;
		//  },
		//  "https://relay.azure.net/.default");

		var tokenCreds = new TokenCredentialAuthenticationProvider(new AzureCliCredential());
		var x = new DefaultAzureCredential(new DefaultAzureCredentialOptions { TenantId = "72aa0d83-624a-4ebf-a683-1b9b45548610" });
		var token = x.GetToken(new TokenRequestContext());
		// need to get a token! 
		//Assert.That(_graphResource.Connect(token.), Is.Not.Null);
	}

	[Test]
	public void Must_be_able_to_send_message_to_chats()
	{
		var graph = _graphResource.Connect() as IAzTeams;

		var message = new ChatMessage
		{
			Subject = "Test Message",
			Body = new ItemBody { Content = "This is a test, please ignore, thank you", ContentType = BodyType.Text }
		};

		graph.SendMessage(_newOrleansChatId, message);
	}

	public class TokenCredentialAuthenticationProvider : IAuthenticationProvider
	{
		private readonly TokenCredential _tokenCredential;

		public TokenCredentialAuthenticationProvider(TokenCredential tokenCredential)
		{
			_tokenCredential = tokenCredential;
		}

		public async Task AuthenticateRequestAsync(HttpRequestMessage request)
		{
			var accessToken = await _tokenCredential.GetTokenAsync(new TokenRequestContext(new[] { "https://graph.microsoft.com" }), CancellationToken.None);
			request.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken.Token);
		}

		public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}
