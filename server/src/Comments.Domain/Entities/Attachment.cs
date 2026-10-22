using Comments.Domain.Enums;

namespace Comments.Domain.Entities;

public class Attachment(
    AttachmentType type,
    string originalName,
    string storedName,
    string contentType,
    int sizeBytes,
    int? width,
    int? height
)
{
    public int Id { get; private set; }
    public int CommentId { get; private set; }
    public AttachmentType Type { get; private set; } = type;
    public string OriginalName { get; private set; } = originalName;
    public string StoredName { get; private set; } = storedName;
    public string ContentType { get; private set; } = contentType;
    public int SizeBytes { get; private set; } = sizeBytes;
    public int? Width { get; private set; } = width;
    public int? Height { get; private set; } = height;
}