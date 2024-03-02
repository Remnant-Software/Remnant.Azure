using Azure.Storage.Queues;
using Remnant.Azure.Core;
using Remnant.Azure.Storage;
using Remnant.Azure.Tests.TestData;
using Remnant.Dependency.Injector;
using Remnant.Dependency.Interface;

namespace Remnant.Azure.Tests
{
    public class TestFactory
	{
		private readonly string _endPoint;
		private readonly string _queueName;

		public TestFactory()
		{
			_endPoint = @"";
			_queueName = "dev-neill-teams-alerts-inbox";
		}

		[Test]
		public void Must_be_able_to_register_a_resource()
		{
			var factory = new AzFactory();

			factory
				.Register<AzQueue, IAzResource<IAzQueue, IAzQueueConfig>>("MyQueue")
				.UseEndPoint(_endPoint)
				.UseQueue(_queueName)
				.UseEncoding(QueueMessageEncoding.Base64);

			var queue = factory
				.Get<AzQueue>("MyQueue")
				.Connect();

			var messages = queue.PeekMessages();
		}

		[Test]
		public void Must_be_able_to_register_a_resource_and_inject_using_di()
		{
			DIContainer.Create("Remnant.Platform.DIContainer");

			var queue = AzQueue
				.Create()
				.UseEndPoint(_endPoint)
				.UseQueue(_queueName)
				.UseEncoding(QueueMessageEncoding.Base64)
				.Connect();

			DIContainer.Register<AzQueue>(queue, _queueName);
			DIContainer.Register<IAzQueue>(queue, "MyQueue");

			//DIContainer.Resolve<IAzQueue>("MyQueue");

			var testInject = new TestQueueInject();
			var message = testInject.ReadMessage();

		}
	}
}
