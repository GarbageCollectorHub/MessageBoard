using FluentValidation;
using MessageBoard.Application.Comments.Exceptions;
using MessageBoard.Domain.Entities;

namespace MessageBoard.Application.Comments;

public sealed class CommentService(
                ICommentRepository commentRepository, 
                IValidator<CreateCommentRequestDto> validator
                )
{
    public async Task<long> CreateAsync(CreateCommentRequestDto request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await validator.ValidateAndThrowAsync(request, ct);

        if (request.ParentId is long parentId && 
            !await commentRepository.ExistsByIdAsync(parentId, ct))
        {
            throw new ParentCommentNotFoundException(parentId);
        }

        var comment = new Comment
        {
            UserName = request.UserName!,
            Email    = request.Email!,
            HomePage = request.HomePage,
            Text     = request.Text!,
            ParentId = request.ParentId,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        commentRepository.Add(comment);
        await commentRepository.SaveChangesAsync(ct);

        return comment.Id;
    }


}
