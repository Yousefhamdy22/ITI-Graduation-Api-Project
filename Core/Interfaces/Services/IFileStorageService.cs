namespace Core.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(string fileName, Stream fileStream, string folder);
    Task DeleteFileAsync(string filePath);
    Task<string> UpdateFileAsync(string existingFilePath, string newFileName, Stream newFileStream, string OldFolder);
}