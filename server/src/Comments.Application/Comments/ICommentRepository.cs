using Comments.Application.Common;
using Comments.Domain.Entities;

namespace Comments.Application.Comments;

public interface ICommentRepository
{
    Task<IReadOnlyList<Comment>> GetRootPageAsync(
        CommentSortBy sortBy,
        SortDirection direction,
        int skip,
        int take,
        CancellationToken cancellationToken
    );

    Task<int> CountRootsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Comment>> GetRepliesByRootIdsAsync(
        IReadOnlyCollection<int> rootIds,
        CancellationToken cancellationToken
    );
}