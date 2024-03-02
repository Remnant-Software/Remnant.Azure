using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace Remnant.Azure.Storage;

public class AzFileShare : AzStorage<ShareServiceClient, IAzFileShare, IAzFileShareConfig>, IAzFileShare, IAzFileShareConfig
{
	private BlobClientOptions _options = new();
	private BlobUploadOptions _uploadOptions = new();
	private ShareClient? _fileShare;
	private string _fileShareName;

	private AzFileShare()
	{
		_uploadOptions.HttpHeaders = new BlobHttpHeaders();
	}

	public static IAzFileShareConfig Create()
	{
		return new AzFileShare();
	}

	public override IAzFileShare Connect(TokenCredential? credential = null)
	{
		base.Connect(credential);

		if (!string.IsNullOrEmpty(_fileShareName))
			_fileShare = _client.GetShareClient(_fileShareName);

		return this;
	}

	public void UploadFile(string directory, string fileName, byte[] content)
	{
		var directoryClient = _fileShare.GetDirectoryClient(directory);
		directoryClient.CreateIfNotExists();

		var fileClient = directoryClient.GetFileClient(fileName);

		using var stream = new MemoryStream(content);
		fileClient.Create(stream.Length);
		fileClient.UploadRange(new HttpRange(0, stream.Length), stream);
	}

	public IAzFileShareConfig UseFileShare(string fileShareName)
	{
		_fileShareName = fileShareName;
		return this;
	}

	#region Configuration

	public IAzFileShareConfig CreateFileShare(string fileShareName, ShareCreateOptions? createOptions = null, CancellationToken cancellationToken = default)
	{
		object createFileShare(object[] args)
		{
			var response = _client.CreateShare(args[0].ToString(), (ShareCreateOptions)args[1], (CancellationToken)args[2]);
			_fileShare = response.Value;
			return this;
		}

		_actions.Add((createFileShare, new object[] { fileShareName, createOptions, cancellationToken }));
		return this;
	}

	public IAzFileShareConfig DeleteFileShare(string fileShareName, CancellationToken cancellationToken = default)
	{
		object deleteFileShare(object[] args)
		{
			var response = _client.DeleteShare(args[0].ToString(), cancellationToken: (CancellationToken)args[1]);

			if (!string.IsNullOrEmpty(_fileShareName) && _fileShareName.Equals(fileShareName))
				_fileShare = null;

			return this;
		}

		_actions.Add((deleteFileShare, new object[] { fileShareName, cancellationToken }));
		return this;
	}

	#endregion
}
