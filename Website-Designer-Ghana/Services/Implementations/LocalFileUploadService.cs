using Website_Designer_Ghana.Services.Interfaces;

namespace Website_Designer_Ghana.Services.Implementations;

public class LocalFileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalFileUploadService> _logger;
    private readonly string _uploadBasePath;
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };

    public LocalFileUploadService(IWebHostEnvironment environment, ILogger<LocalFileUploadService> logger)
    {
        _environment = environment;
        _logger = logger;
        _uploadBasePath = Path.Combine(_environment.WebRootPath, "uploads");

        // Ensure uploads directory exists
        if (!Directory.Exists(_uploadBasePath))
        {
            Directory.CreateDirectory(_uploadBasePath);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder = "uploads")
    {
        try
        {
            // Sanitize filename
            var sanitizedFileName = SanitizeFileName(fileName);
            var uniqueFileName = GenerateUniqueFileName(sanitizedFileName);

            // Create folder path
            var folderPath = Path.Combine(_uploadBasePath, folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Full file path
            var filePath = Path.Combine(folderPath, uniqueFileName);

            // Save file
            using (var fileStreamOut = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStreamOut);
            }

            // Return relative path
            return $"/uploads/{folder}/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string folder = "images")
    {
        // Validate image extension
        if (!IsValidImageExtension(fileName))
        {
            throw new ArgumentException("Invalid image file type. Allowed types: jpg, jpeg, png, gif, webp, svg");
        }

        return await UploadFileAsync(imageStream, fileName, folder);
    }

    public Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            return Task.FromResult(false);
        }
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
            return Task.FromResult(File.Exists(fullPath));
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<byte[]?> GetFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                return await File.ReadAllBytesAsync(fullPath);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file: {FilePath}", filePath);
            return null;
        }
    }

    public Task<long> GetFileSizeAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                var fileInfo = new FileInfo(fullPath);
                return Task.FromResult(fileInfo.Length);
            }
            return Task.FromResult(0L);
        }
        catch
        {
            return Task.FromResult(0L);
        }
    }

    public string GetFileUrl(string filePath)
    {
        return filePath.StartsWith("/") ? filePath : $"/{filePath}";
    }

    public bool IsValidImageExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedImageExtensions.Contains(extension);
    }

    public bool IsValidFileSize(long fileSize, long maxSizeInMB = 10)
    {
        var maxSizeInBytes = maxSizeInMB * 1024 * 1024;
        return fileSize <= maxSizeInBytes;
    }

    private string SanitizeFileName(string fileName)
    {
        // Remove invalid characters
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

        // Remove spaces and special characters
        sanitized = sanitized.Replace(" ", "_");

        return sanitized.ToLowerInvariant();
    }

    private string GenerateUniqueFileName(string fileName)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var randomString = Guid.NewGuid().ToString("N").Substring(0, 8);

        return $"{fileNameWithoutExtension}_{timestamp}_{randomString}{extension}";
    }
}
