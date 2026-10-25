using System.ComponentModel.DataAnnotations;

using Comments.Api.Models;
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

    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create(
        [FromForm] CreateCommentForm form,
        CancellationToken cancellationToken = default
    )
    {
        string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        string userAgent = Request.Headers.UserAgent.ToString();

        CreateCommentRequest request = new(
            ParentId: form.ParentId,
            UserName: form.UserName,
            Email: form.Email,
            HomePage: form.HomePage,
            Text: form.Text,
            IpAddress: clientIp,
            UserAgent: userAgent
        );

        CommentDto created = await _commentService.CreateAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPost("preview")]
    public ActionResult<PreviewTextResponse> Preview(PreviewTextRequest request)
    {
        return Ok(new PreviewTextResponse(_commentService.Preview(request.Text)));
    }
}