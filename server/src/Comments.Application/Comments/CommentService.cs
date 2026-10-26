using Comments.Application.Captcha;
using Comments.Application.Comments.Dtos;
using Comments.Application.Comments.Text;
using Comments.Application.Common;
using Comments.Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

namespace Comments.Application.Comments;

public class CommentService(
    ICommentRepository repository,
    IValidator<CreateCommentRequest> validator,
    ICommentTextSanitizer sanitizer,
    ICaptchaService captchaService
) : ICommentService
{
    private readonly ICommentRepository _repository = repository;
    private readonly IValidator<CreateCommentRequest> _validator = validator;
    private readonly ICommentTextSanitizer _sanitizer = sanitizer;
    private readonly ICaptchaService _captchaService = captchaService;
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

    public async Task<CommentDto> CreateAsync(CreateCommentRequest request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        if (!_captchaService.Validate(request.CaptchaId, request.CaptchaCode))
        {
            throw new ValidationException([
                new ValidationFailure(nameof(CreateCommentRequest.CaptchaCode), "Invalid or expired captcha code")
            ]);
        }

        string html = SanitizeOrThrow(request.Text);
        int? rootId = null;
        if (request.ParentId is not null)
        {
            Comment parent = await _repository.GetByIdAsync(request.ParentId.Value, cancellationToken)
                ?? throw new ValidationException([
                    new ValidationFailure(nameof(CreateCommentRequest.ParentId), "Parent comment does not exist.")
                ]);
            rootId = parent.RootId ?? parent.Id;
        }

        string userAgent = request.UserAgent.Length > Comment.UserAgentMaxLength
            ? request.UserAgent[..Comment.UserAgentMaxLength]
            : request.UserAgent;

        Comment comment = new(
            parentId: request.ParentId,
            rootId: rootId,
            userName: request.UserName,
            email: request.Email,
            homePage: request.HomePage,
            text: html,
            createdAt: DateTime.UtcNow,
            ipAddress: request.IpAddress,
            userAgent
        );
        await _repository.AddAsync(comment, cancellationToken);

        var emptyLookup = Array.Empty<Comment>().ToLookup(c => c.ParentId);
        return ToDto(comment, emptyLookup);
    }

    public string Preview(string text) => SanitizeOrThrow(text);

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

    private string SanitizeOrThrow(string text)
    {
        var sanitizeResult = _sanitizer.Sanitize(text);

        if (!sanitizeResult.IsValid)
        {
            throw new ValidationException([
                new ValidationFailure(nameof(CreateCommentRequest.Text), sanitizeResult.Error)
            ]);
        }

        return sanitizeResult.Html;
    }
}