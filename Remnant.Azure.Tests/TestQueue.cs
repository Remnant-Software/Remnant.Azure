using Azure.Storage.Queues;
using Remnant.Azure.Core;
using Remnant.Azure.Storage;
using System.Net;

namespace Remnant.Azure.Tests
{
    public class TestQueue
	{
		private readonly string _endPoint;
		private readonly string _queueName;
		private IAzResource<IAzQueue, IAzQueueConfig> _queueResource;

		public TestQueue()
		{
			_endPoint = @"";
			_queueName = "dev-neill-teams-alerts-inbox";
		}

		[SetUp]
		public void Setup()
		{
			_queueResource = AzQueue
				.Create()
				.UseEndPoint(_endPoint)
				.UseEncoding(QueueMessageEncoding.Base64)
				.UseQueue(_queueName)
				.Resource;
		}

		[Test]
		public void Must_be_able_to_connect()
		{
			Assert.That(_queueResource.Connect(), Is.Not.Null);
		}

		[Test]
		public void Must_be_able_to_create_and_delete_queue()
		{
			_queueResource
				.UseEndPoint(_endPoint)
				.CreateQueue("test-queue")
				.DeleteQueue("test-queue")
				.Connect();
		}

		[Test]
		public void Must_be_able_to_delete_all_message()
		{
			var queue = _queueResource.Connect();
			var response = queue.DeleteAllMessages();

			Assert.That(
				response.Status == (int)HttpStatusCode.OK ||
				response.Status == (int)HttpStatusCode.NoContent);
		}

		[Test]
		public void Must_be_able_to_send_message()
		{
			var queue = _queueResource.Connect();
			var (message, response) = queue.SendMessage("Hallo Marisca...");

			Assert.That(message, Is.Not.Null);
			Assert.That(response, Is.Not.Null);
		}

		[Test]
		public void Must_be_able_to_peek_message()
		{
			var queue = _queueResource.Connect();

			queue.DeleteAllMessages();
			var (message, _) = queue.SendMessage("Hallo Marisca...again");
			var (messages, response) = queue.PeekMessages();

			Assert.That(response, Is.Not.Null);
			Assert.That(messages, Is.Not.Null);
			Assert.That(messages.Count == 1);
			Assert.That(messages[0].MessageId == message.MessageId);
			Assert.That(messages[0].MessageText == "Hallo Marisca...again");
		}

		[Test]
		public void Must_be_able_to_read_messages()
		{
			var queue = _queueResource.Connect();

			queue.DeleteAllMessages();
			var (message, _) = queue.SendMessage("Hallo Marisca...hallo again");
			var (messages, response) = queue.ReadMessages();

			Assert.That(response, Is.Not.Null);
			Assert.That(messages, Is.Not.Null);
			Assert.That(messages.Count == 1);
			Assert.That(messages[0].MessageId == message.MessageId);
			Assert.That(messages[0].MessageText == "Hallo Marisca...hallo again");
		}

		[Test]
		public void Must_be_able_to_read_message()
		{
			var queue = _queueResource.Connect();

			queue.DeleteAllMessages();
			var (sendMessage, _) = queue.SendMessage("Hallo Marisca...hallo again");
			var (readMessage, response) = queue.ReadMessage();

			Assert.That(response, Is.Not.Null);
			Assert.That(readMessage, Is.Not.Null);
			Assert.That(readMessage.MessageId == sendMessage.MessageId);
			Assert.That(readMessage.MessageText == "Hallo Marisca...hallo again");
		}

		[Test]
		public void Must_be_able_to_update_message()
		{
			var queue = _queueResource.Connect();

			queue.DeleteAllMessages();
			var (message, _) = queue.SendMessage("Hallo...Marisa..are you there?");
			var (updatedMessage, response) = queue.UpdateMessage(message.MessageId, message.PopReceipt, "Marisca...Marisca..where are you?");

			Assert.That(response, Is.Not.Null);
			Assert.That(updatedMessage, Is.Not.Null);

			var (readMessage, _) = queue.ReadMessage();
			Assert.That(readMessage.MessageText == "Marisca...Marisca..where are you?");

		}
	}
}
