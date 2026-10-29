using Comments.Application.Comments.Dtos;
using Comments.Application.Common;

namespace Comments.Application.Comments;

public interface ICommentService
{
    Task<PagedResponse<CommentDto>> GetPageAsync(
        int page,
        CommentSortBy sortBy,
        SortDirection direction,
        CancellationToken cancellationToken
    );

    Task<CommentDto> CreateAsync(
        CreateCommentRequest request,
        CancellationToken cancellationToken
    );

    string Preview(string text);
}