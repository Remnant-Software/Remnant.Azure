//using Azure.Storage.Blobs.Models;
//using Remnant.Azure.Storage;
//using Remnant.Azure.Storage.Interfaces;
//using Remnant.Framework.Services;

//namespace Remnant.Azure.Tests
//{
//	public class CopyContainer
//	{
//		private readonly string _connectionString;
//		private readonly string _containerName;
//		private IAzResource<IAzContainer, IAzContainerConfig> _containerService;

//		public CopyContainer()
//		{
//			_connectionString = @"";
//			_containerName = "";
//		}

//		[SetUp]
//		public void Setup()
//		{
//			_containerService = AzContainer
//				.Create()
//				.UseContainer(_containerName)
//				.UseEndPoint(_connectionString);
//		}

//		[Test]
//		public void Must_be_able_to_connect()
//		{
//			Assert.That(_containerService.Connect(), Is.Not.Null);
//		}


//		[Test]
//		public async Task Must_be_able_to_download_blobs()
//		{
//			var container = _containerService.Connect();
//			var blobs = container.ReadBlobs("2023/01");
//			var targetDir = @"c:\temp\Spain\";

//			var blobItems = new List<BlobItem>();

//			foreach (var blob in blobs)
//			{
//				blobItems.Add(blob);
//			}

//			await container.DownloadBlobs(blobItems, 10,
//			(blobItem, blobContent) =>
//			{
//				var path = IOService.ExtractDirectory(blobItem.Name);
//				var file = IOService.ExtractFileName(blobItem.Name);
//				var dir = $@"{targetDir}{path}\";

//				IOService.EnforceDirectory(dir);

//				File.WriteAllText(dir+file, blobContent);
//			});

//			Console.Write(Environment.NewLine);
//		}
//	}
//}
