using Comments.Application.Comments;
using Comments.Application.Common;
using Comments.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Comments.Infrastructure.Persistence.Repositories;

public class CommentRepository(CommentsDbContext db) : ICommentRepository
{
    private readonly CommentsDbContext _db = db;

    public Task<int> CountRootsAsync(CancellationToken cancellationToken)
    {
        return _db.Comments.CountAsync(c => c.ParentId == null, cancellationToken);
    }
    public async Task<IReadOnlyList<Comment>> GetRootPageAsync(
        CommentSortBy sortBy,
        SortDirection direction,
        int skip,
        int take,
        CancellationToken cancellationToken
    )
    {
        IQueryable<Comment> query = _db.Comments
            .AsNoTracking()
            .Include(c => c.Attachment)
            .Where(c => c.ParentId == null);

        IOrderedQueryable<Comment> ordered = (sortBy, direction) switch
        {
            (CommentSortBy.CreatedAt, SortDirection.Asc) => query.OrderBy(c => c.CreatedAt),
            (CommentSortBy.CreatedAt, SortDirection.Desc) => query.OrderByDescending(c => c.CreatedAt),
            (CommentSortBy.UserName, SortDirection.Asc) => query.OrderBy(c => c.UserName),
            (CommentSortBy.UserName, SortDirection.Desc) => query.OrderByDescending(c => c.UserName),
            (CommentSortBy.Email, SortDirection.Asc) => query.OrderBy(c => c.Email),
            (CommentSortBy.Email, SortDirection.Desc) => query.OrderByDescending(c => c.Email),
            _ => throw new ArgumentOutOfRangeException(nameof(sortBy))
        };

        return await ordered
            .ThenBy(c => c.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Comment>> GetRepliesByRootIdsAsync(
        IReadOnlyCollection<int> rootIds,
        CancellationToken cancellationToken
    )
    {
        return await _db.Comments
            .AsNoTracking()
            .Include(c => c.Attachment)
            .Where(c => c.RootId != null && rootIds.Contains(c.RootId.Value))
            .ToListAsync(cancellationToken);
    }
}