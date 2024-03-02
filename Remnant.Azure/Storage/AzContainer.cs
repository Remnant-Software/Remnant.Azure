using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Net;

namespace Remnant.Azure.Storage;

public class AzContainer : AzStorage<BlobServiceClient, IAzContainer, IAzContainerConfig>, IAzContainer, IAzContainerConfig
{
	private BlobClientOptions _options = new();
	private BlobUploadOptions _uploadOptions = new();
	private BlobContainerClient? _container;
	private string _containerName;

	private AzContainer()
	{
		_uploadOptions.HttpHeaders = new BlobHttpHeaders();
	}

	public static IAzContainerConfig Create()
	{
		return new AzContainer();
	}

	public IAzContainer ContentType(string contentType)
	{
		_uploadOptions.HttpHeaders.ContentType = contentType;
		return this;
	}

	public override IAzContainer Connect(TokenCredential? credential = null)
	{
		base.Connect(credential);

		if (!string.IsNullOrEmpty(_containerName))
			_container = _client.GetBlobContainerClient(_containerName);

		return this;
	}

	public byte[] GetBlobTagResult(Uri uri)
	{
		return null;
	}

	public IAzContainerConfig UseContainer(string containerName)
	{
		//Shield
		//	.AgainstNullOrEmpty(containerName)
		//	.Raise<ContainerException>(ContainerException.NoContainerName);

		_containerName = containerName;
		return this;
	}

	public int DeleteAllBlobs()
	{
		throw new NotImplementedException(); //safeguard for now
		var blobs = ReadBlobs();
		foreach (var blob in blobs)
		{
			DeleteBlob(blob.Name);
		}

		return blobs.Count();
	}

	public async Task DownloadBlobs(List<BlobItem> blobs, int maxParallelDownloads, Action<BlobItem, string> onDownLoaded, CancellationToken cancellation = default)
	{
		//Shield.AgainstNull(onDownLoaded).Raise();

		var semaphore = new SemaphoreSlim(maxParallelDownloads);
		var tasks = new List<Task>();

		foreach (BlobItem blob in blobs)
		{
			await semaphore.WaitAsync();

			tasks.Add(Task.Run(async () =>
			{
				try
				{
					var blobClient = _container.GetBlobClient(blob.Name);
					var response = await blobClient.DownloadAsync();

					using (var writer = new MemoryStream())
					{
						await response.Value.Content.CopyToAsync(writer);
						//onDownLoaded.Invoke(blob, writer.ToText()); // Todo: reimplement
					}
				}
				finally
				{
					semaphore.Release();
				}
			}));
		}

		await Task.WhenAll(tasks);

	}

	public Pageable<BlobItem> ReadBlobs(string prefix = null, BlobTraits traits = BlobTraits.None, BlobStates states = BlobStates.None, CancellationToken cancellation = default)
	{
		return _container.GetBlobs(traits, states, prefix, cancellation);
	}

	public async Task<string> ReadBlob(string blobName)
	{
		var blobClient = _container.GetBlobClient(blobName);
		BlobDownloadInfo blob = await blobClient.DownloadAsync();

		using var reader = new StreamReader(blob.Content);
		return await reader.ReadToEndAsync();
	}

	public byte[] ReadBinaryBlob(string blobName)
	{
		var blobClient = _container.GetBlobClient(blobName);

		using var stream = new MemoryStream();
		blobClient.DownloadTo(stream);
		return stream.ToArray();
	}

	public (BlobContentInfo, Response<BlobContentInfo>) SaveBlob(string blobName, byte[] blob, CancellationToken cancellationToken = default)
	{
		using var stream = new MemoryStream(blob);
		return SaveBlob(blobName, stream, cancellationToken);
	}

	public (BlobContentInfo, Response<BlobContentInfo>) SaveBlob(string blobName, BinaryData blob, CancellationToken cancellationToken = default)
	{
		var response = _container.UploadBlob(blobName, blob, cancellationToken);
		return (response.Value, response);
	}

	public (BlobContentInfo, Response<BlobContentInfo>) SaveBlob(string blobName, Stream blob, CancellationToken cancellationToken = default)
	{
		var blobClient = _container.GetBlobClient(blobName);
		var response = blobClient.Upload(blob, _uploadOptions, cancellationToken);
		return (response.Value, response);
	}

	public Response<bool> DeleteBlob(string blobName, bool ifExists = true, CancellationToken cancellationToken = default)
	{
		if (ifExists)
		{
			return _container.DeleteBlobIfExists(blobName, cancellationToken: cancellationToken);
		}


		var response = _container.DeleteBlob(blobName, cancellationToken: cancellationToken);
		return Response.FromValue(response.Status == (int)HttpStatusCode.Accepted, response);
	}

	public Pageable<TaggedBlobItem> FindBlobsByTags(string tagsExpression, CancellationToken cancellationToken = default)
	{
		return _container.FindBlobsByTags(tagsExpression, cancellationToken);
	}

	#region Configuration

	public IAzContainerConfig CreateContainer(string containerName, Dictionary<string, string>? metaData = null, CancellationToken cancellationToken = default)
	{
		//Shield
		//	.AgainstNullOrEmpty(containerName)
		//	.Raise<ContainerException>(ContainerException.NoContainerName);

		object createContainer(object[] args)
		{
			var response = _client.CreateBlobContainer(args[0].ToString(), PublicAccessType.None, (Dictionary<string, string>)args[1], (CancellationToken)args[2]);
			_container = response.Value;
			return this;
		}

		_actions.Add((createContainer, new object[] { containerName, metaData, cancellationToken }));
		return this;
	}

	public IAzContainerConfig DeleteContainer(string containerName, CancellationToken cancellationToken = default)
	{
		//Shield
		//	.AgainstNullOrEmpty(containerName)
		//	.Raise<ContainerException>(ContainerException.NoContainerName);

		object deleteContainer(object[] args)
		{
			var response = _client.DeleteBlobContainer(args[0].ToString(), cancellationToken: (CancellationToken)args[1]);

			if (!string.IsNullOrEmpty(_containerName) && _containerName.Equals(containerName))
				_container = null;

			return this;
		}

		_actions.Add((deleteContainer, new object[] { containerName, cancellationToken }));
		return this;
	}

	#endregion
}
