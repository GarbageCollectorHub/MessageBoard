using MessageBoard.Application.Comments;
using MessageBoard.Application.Comments.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MessageBoard.Api.Controllers;

[ApiController]
[Route("api/comments")]
public sealed class CommentsController(CommentService commentService) : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<CommentResponseDto>> Create(CreateCommentRequestDto request, CancellationToken ct)
    {
        var response = await commentService.CreateAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, response);
    }


}