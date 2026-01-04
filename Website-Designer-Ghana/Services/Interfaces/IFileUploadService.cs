namespace Website_Designer_Ghana.Services.Interfaces;

public interface IFileUploadService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder = "uploads");
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string folder = "images");
    Task<bool> DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
    Task<byte[]?> GetFileAsync(string filePath);
    Task<long> GetFileSizeAsync(string filePath);
    string GetFileUrl(string filePath);
    bool IsValidImageExtension(string fileName);
    bool IsValidFileSize(long fileSize, long maxSizeInMB = 10);
}
