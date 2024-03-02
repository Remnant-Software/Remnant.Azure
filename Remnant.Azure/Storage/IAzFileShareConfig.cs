using Azure.Storage.Files.Shares.Models;
using Remnant.Azure.Core;

namespace Remnant.Azure.Storage;

public interface IAzFileShareConfig : IAzResource<IAzFileShare, IAzFileShareConfig>
{
	IAzFileShareConfig CreateFileShare(string fileShareName, ShareCreateOptions? createOptions = null, CancellationToken cancellationToken = default);

	IAzFileShareConfig DeleteFileShare(string fileShareName, CancellationToken cancellationToken = default);

	IAzFileShareConfig UseFileShare(string fileShareName);
}