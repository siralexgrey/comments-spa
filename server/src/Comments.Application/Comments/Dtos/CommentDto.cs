namespace Comments.Application.Comments.Dtos;

public record CommentDto(
    int Id,
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    AttachmentDto? Attachment,
    IReadOnlyList<CommentDto> Replies,
    DateTime CreatedAt
);