using Remnant.Azure.Storage;
using Remnant.Dependency.Injector;

namespace Remnant.Azure.Tests.TestData
{
    public partial class TestQueueInject
	{
		[Inject("MyQueue")]
		private readonly IAzQueue _queue;

		[Inject]
		private readonly AzQueue _queue2;

		public string ReadMessage()
		{
			var (message, response) = _queue.ReadMessage();
			return message.MessageText;
		}
	}
}
