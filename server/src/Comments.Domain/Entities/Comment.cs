namespace Comments.Domain.Entities;

public class Comment(
    int? parentId,
    int? rootId,
    string userName,
    string email,
    string? homePage,
    string text,
    DateTime createdAt,
    string ipAddress,
    string userAgent
)
{
    public const int UserNameMaxLength = 50;
    public const int EmailMaxLength = 254;
    public const int HomePageMaxLength = 2048;
    public const int IpAddressMaxLength = 45;
    public const int UserAgentMaxLength = 512;
    
    public int Id { get; private set; }
    public int? ParentId { get; private set; } = parentId;
    public int? RootId { get; private set; } = rootId;
    public string UserName { get; private set; } = userName;
    public string Email { get; private set; } = email;
    public string? HomePage { get; private set; } = homePage;
    public string Text { get; private set; } = text;
    public DateTime CreatedAt { get; private set; } = createdAt;
    public string IpAddress { get; private set; } = ipAddress;
    public string UserAgent { get; private set; } = userAgent;
    public Attachment? Attachment { get; private set; }

    public void AttachFile(Attachment attachment)
    {
        Attachment = attachment;
    }
}