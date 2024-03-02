using Microsoft.Graph.Models;
using Remnant.Azure.Core;

namespace Remnant.Azure.Graph;

public interface IAzTeams : IAzure
{
	void SendMessage(string teamId, string channelId, ChatMessage chatMessage);
	void SendMessage(string chatId, ChatMessage chatMessage);
}
