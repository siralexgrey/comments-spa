using System.ComponentModel.DataAnnotations;

using Comments.Application.Comments;
using Comments.Application.Comments.Dtos;
using Comments.Application.Common;

using Microsoft.AspNetCore.Mvc;

namespace Comments.Api.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    private readonly ICommentService _commentService = commentService;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<CommentDto>>> GetPage(
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery] CommentSortBy sortBy = CommentSortBy.CreatedAt,
        [FromQuery] SortDirection dir = SortDirection.Desc,
        CancellationToken cancellationToken = default
    )
    {
        return await _commentService.GetPageAsync(page, sortBy, dir, cancellationToken);
    }
}