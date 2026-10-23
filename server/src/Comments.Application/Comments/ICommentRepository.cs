using Comments.Application.Common;
using Comments.Domain.Entities;

namespace Comments.Application.Comments;

public interface ICommentRepository
{
    Task<int> CountRootsAsync(CancellationToken cancellationToken);
    Task<Comment?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<Comment>> GetRootPageAsync(
        CommentSortBy sortBy,
        SortDirection direction,
        int skip,
        int take,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<Comment>> GetRepliesByRootIdsAsync(
        IReadOnlyCollection<int> rootIds,
        CancellationToken cancellationToken
    );
    Task AddAsync(Comment comment, CancellationToken cancellationToken);
}