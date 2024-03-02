using Azure.Core;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Remnant.Azure.Core;

namespace Remnant.Azure.Graph;

public class AzGraph : AzResource<IAzGraph, IAzGraph>, IAzGraph, IAzTeams
{
	private GraphServiceClient _client;

	private AzGraph() { }

	public static IAzGraph Create()
	{
		return new AzGraph();
	}

	public override IAzGraph Connect(TokenCredential? credential = null)
	{
		_client = new GraphServiceClient(credential);
		return this;
	}

	public void SendMessage(string teamId, string channelId, ChatMessage chatMessage)
	{
		var x = _client.Teams[teamId];
		var y = x.Channels[channelId];

		y.Messages.PostAsync(chatMessage).Wait();

	}

	public void SendMessage(string chatId, ChatMessage chatMessage)
	{
		_client.Chats[chatId].Messages.PostAsync(chatMessage).Wait();
	}
}
