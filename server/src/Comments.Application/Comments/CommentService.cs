using Comments.Application.Comments.Dtos;
using Comments.Application.Common;
using Comments.Domain.Entities;

namespace Comments.Application.Comments;

public class CommentService(ICommentRepository repository) : ICommentService
{
    private readonly ICommentRepository _repository = repository;
    private const int PageSize = 25;
    public async Task<PagedResponse<CommentDto>> GetPageAsync(
        int page,
        CommentSortBy sortBy,
        SortDirection direction,
        CancellationToken cancellationToken
    )
    {
        int totalCount = await _repository.CountRootsAsync(cancellationToken);
        int skip = (page - 1) * PageSize;

        IReadOnlyList<Comment> rootCommentsList = await _repository.GetRootPageAsync(
            sortBy, direction, skip, PageSize, cancellationToken
        );
        if (rootCommentsList.Count == 0) return new PagedResponse<CommentDto>([], page, PageSize, totalCount);
        IReadOnlyList<int> rootIds = [.. rootCommentsList.Select(r => r.Id)];
        IReadOnlyList<Comment> replies = await _repository.GetRepliesByRootIdsAsync(rootIds, cancellationToken);
        ILookup<int?, Comment> repliesByParent = replies.ToLookup(r => r.ParentId);
        return new PagedResponse<CommentDto>([.. rootCommentsList.Select(r => ToDto(r, repliesByParent))], page, PageSize, totalCount);
    }

    private static CommentDto ToDto(Comment comment, ILookup<int?, Comment> repliesByParent)
    {
        return new CommentDto(
            Id: comment.Id,
            UserName: comment.UserName,
            Email: comment.Email,
            HomePage: comment.HomePage,
            Text: comment.Text,
            Replies: [.. repliesByParent[comment.Id].OrderBy(r => r.CreatedAt).Select(r => ToDto(r, repliesByParent))],
            Attachment: ToDto(comment.Attachment),
            CreatedAt: comment.CreatedAt
        );

    }
    private static AttachmentDto? ToDto(Attachment? attachment)
    {
        if (attachment is null) return null;
        return new AttachmentDto(
            Id: attachment.Id,
            OriginalName: attachment.OriginalName,
            Type: attachment.Type,
            Url: $"/uploads/{attachment.StoredName}",
            Width: attachment.Width,
            Height: attachment.Height
        );
    }
}