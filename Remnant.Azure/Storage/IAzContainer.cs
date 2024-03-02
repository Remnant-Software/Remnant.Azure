using Azure;
using Azure.Storage.Blobs.Models;

namespace Remnant.Azure.Storage;

public interface IAzContainer : IAzStorage
{
	IAzContainer ContentType(string contentType);

	Pageable<BlobItem> ReadBlobs(string prefix = null, BlobTraits traits = BlobTraits.None, BlobStates states = BlobStates.None, CancellationToken cancellation = default);

	Task DownloadBlobs(List<BlobItem> blobNames, int maxParallelDownloads, Action<BlobItem, string> onDownLoaded, CancellationToken cancellation = default);

	int DeleteAllBlobs();

	Task<string> ReadBlob(string blobName);

	byte[] ReadBinaryBlob(string blobName);

	(BlobContentInfo, Response<BlobContentInfo>) SaveBlob(string blobName, byte[] blob, CancellationToken cancellationToken = default);

	(BlobContentInfo, Response<BlobContentInfo>) SaveBlob(string blobName, BinaryData blob, CancellationToken cancellationToken = default);

	(BlobContentInfo, Response<BlobContentInfo>) SaveBlob(string blobName, Stream blob, CancellationToken cancellationToken = default);

	Response<bool> DeleteBlob(string blobName, bool ifExists = true, CancellationToken cancellationToken = default);

	Pageable<TaggedBlobItem> FindBlobsByTags(string tagsExpression, CancellationToken cancellationToken = default);
}