using Comments.Application.Attachments;

namespace Comments.Infrastructure.Attachments;

public class LocalFileStorage : IFileStorage
{
    private readonly string _rootPath;

    public LocalFileStorage(string rootPath)
    {
        Directory.CreateDirectory(rootPath);
        _rootPath = rootPath;
    }

    public async Task<string> SaveAsync(byte[] content, string extension, CancellationToken cancellationToken)
    {
        string name = $"{Guid.NewGuid():N}{extension}";
        string path = Path.Combine(_rootPath, name);

        await File.WriteAllBytesAsync(path, content, cancellationToken);
        return name;
    }
}