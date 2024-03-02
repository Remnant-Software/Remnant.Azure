using Remnant.Azure.Core;
using Remnant.Azure.Storage;

namespace Remnant.Azure.Tests
{
    public class TestContainer
	{
		private readonly string _connectionString;
		private readonly string _containerName;
		private IAzResource<IAzContainer, IAzContainerConfig> _containerResource;

		public TestContainer()
		{

			_connectionString = @"";
			_containerName = "dev-neill-container";
		}

		[SetUp]
		public void Setup()
		{
			_containerResource = AzContainer
				.Create()
				.UseContainer(_containerName)
				.UseEndPoint(_connectionString);

			//_containerService.Connect().DeleteAllBlobs();
		}

		[Test]
		public void Must_be_able_to_connect()
		{
			Assert.That(_containerResource.Connect(), Is.Not.Null);
		}

		[Test]
		public void Must_be_able_to_create_and_delete_container()
		{
			// NOTE: delete happens async by other azure backend job which takes it time
			_containerResource
				.UseEndPoint(_connectionString)
				.CreateContainer("test-container")
				.DeleteContainer("test-container")
				.Connect();
		}

		[Test]
		public void Must_be_able_to_save_blob()
		{
			var container = _containerResource.Connect();
			var (blobInfo, response) = container.SaveBlob(@"test-blob1", new BinaryData("Hey Marisca!"));

			Assert.That(response, Is.Not.Null);
			Assert.That(blobInfo, Is.Not.Null);
		}

		[Test]
		public void Must_be_able_to_read_blobs()
		{
			var container = _containerResource.Connect();

			var (blobInfo, response) = container.SaveBlob("test-blob2", new BinaryData("Hey Marisca! Hallo!"));
			var blobs = container.ReadBlobs();

			var blob = blobs.FirstOrDefault();
			Assert.That(blobs, Is.Not.Null);
			Assert.That(blob, Is.Not.Null);
			Assert.That(blob.Name, Is.Not.Empty);
		}

		[Test]
		public void Must_be_able_to_read_blob()
		{
			var container = _containerResource.Connect();

			container.SaveBlob("test-blob3", new BinaryData("Marisca..."));
			var blob = container.ReadBlob("test-blob3").Result;

			Assert.That(blob, Is.Not.Null);
			Assert.That(blob == "Marisca...");
		}

		[Test]
		public void Must_be_able_to_delete_blob()
		{
			var container = _containerResource.Connect();

			container.SaveBlob("test-blob4", new BinaryData("Marisca...oh nooo!"));
			var response = container.DeleteBlob("test-blob4");

			Assert.That(response, Is.Not.Null);
			Assert.That(response.Value == true);
		}
	}
}
