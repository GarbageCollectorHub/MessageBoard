using FluentValidation;
using MessageBoard.Application.Comments.DTOs;
using MessageBoard.Application.Comments.Exceptions;
using MessageBoard.Domain.Entities;

namespace MessageBoard.Application.Comments;

public sealed class CommentService(
                ICommentRepository commentRepository, 
                IValidator<CreateCommentRequestDto> validator,
                IValidator<GetCommentsRequestDto> getCommentsValidator
                )
{

    public async Task<CommentPageResponseDto> GetRootPageAsync(GetCommentsRequestDto request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await getCommentsValidator.ValidateAndThrowAsync(request, ct);

        return await commentRepository.GetRootPageAsync(request, ct);
    }


    public async Task<CommentResponseDto> CreateAsync(CreateCommentRequestDto request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await validator.ValidateAndThrowAsync(request, ct);

        // Проверяем существование родителя, чтобы вернуть 404, а не ошибку внешнего ключа из БД
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

        return new CommentResponseDto(
            comment.Id,
            comment.UserName,
            comment.HomePage,
            comment.Text,
            comment.ParentId,
            comment.CreatedAtUtc);

    }


}
