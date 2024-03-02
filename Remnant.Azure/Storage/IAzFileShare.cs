namespace Remnant.Azure.Storage;

public interface IAzFileShare : IAzStorage
{
	void UploadFile(string directory, string fileName, byte[] content);
}