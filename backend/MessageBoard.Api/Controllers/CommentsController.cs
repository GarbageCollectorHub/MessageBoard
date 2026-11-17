using MessageBoard.Application.Comments;
using MessageBoard.Application.Comments.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MessageBoard.Api.Controllers;

[ApiController]
[Route("api/comments")]
public sealed class CommentsController(CommentService commentService) : ControllerBase
{

    [HttpGet]
    [EndpointDescription("Returns up to 25 root comments per page. By default, returns page 1 with the newest comments first.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CommentPageResponseDto>> Get([FromQuery] GetCommentsRequestDto request, CancellationToken ct)
    {
        CommentPageResponseDto response = await commentService.GetRootPageAsync(request, ct);

        return Ok(response);
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]   // parentId not found
    public async Task<ActionResult<CommentResponseDto>> Create(CreateCommentRequestDto request, CancellationToken ct)
    {
        var response = await commentService.CreateAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, response);
    }


}