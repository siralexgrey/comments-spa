namespace Comments.Application.Attachments;

public interface IFileStorage
{
    Task<string> SaveAsync(byte[] content, string extension, CancellationToken cancellationToken);
}