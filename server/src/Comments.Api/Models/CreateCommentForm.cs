namespace Comments.Api.Models;

public record CreateCommentForm(
    int? ParentId,
    string UserName,
    string Email,
    string? HomePage,
    string Text
);