namespace Comments.Application.Comments.Dtos;

public record CreateCommentRequest(
    int? ParentId,
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    string IpAddress,
    string UserAgent
);