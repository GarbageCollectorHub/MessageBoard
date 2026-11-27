using FluentValidation;
using FluentValidation.Results;
using MessageBoard.Application.Comments.DTOs;
using MessageBoard.Application.Comments.Exceptions;
using MessageBoard.Application.Comments.Queries;
using MessageBoard.Domain.Entities;
using System.Globalization;

namespace MessageBoard.Application.Comments;

public sealed class CommentService(
                ICommentRepository commentRepository, 
                IValidator<CreateCommentRequestDto> validator,
                IValidator<GetCommentsRequestDto> getCommentsValidator
                )
{

    private const int ReplyBatchSize = 10;

    public async Task<CommentPageResponseDto> GetRootPageAsync(GetCommentsRequestDto request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await getCommentsValidator.ValidateAndThrowAsync(request, ct);

        return await commentRepository.GetRootPageAsync(request, ct);
    }


    public async Task<CommentRepliesResponseDto> GetRepliesAsync(long parentId, string? cursor,
        CancellationToken ct = default)
    {
        if (parentId <= 0)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(parentId), "Parent ID must be greater than zero.")
            });
        }

        CommentReplyCursor? replyCursor = ParseReplyCursor(cursor);

        if (!await commentRepository.ExistsByIdAsync(parentId, ct))
        {
            throw new ParentCommentNotFoundException(parentId);
        }

        // Запрашиваем 11 ответов: дополнительный нужен, чтобы определить наличие следующей порции
        IReadOnlyList<CommentResponseDto> replies = await commentRepository.GetRepliesAsync(
            parentId, replyCursor, ReplyBatchSize + 1, ct);

        if (replies.Count <= ReplyBatchSize)
        {
            return new CommentRepliesResponseDto(replies, null);
        }

        CommentResponseDto[] items = replies.Take(ReplyBatchSize).ToArray();
        CommentResponseDto lastReply = items[ReplyBatchSize - 1];

        // Курсор указывает на 10-й ответ, чтобы следующий запрос начался с 11-го
        string nextCursor = string.Create(
            CultureInfo.InvariantCulture, $"{lastReply.CreatedAtUtc.UtcTicks}.{lastReply.Id}");

        return new CommentRepliesResponseDto(items, nextCursor);
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

    // === Helpers ===

    private static CommentReplyCursor? ParseReplyCursor(string? cursor)
    {
        // Без курсора загружаем первые ответы в ветке.
        if (string.IsNullOrWhiteSpace(cursor))
        {
            return null;
        }

        // Клиент передаёт nextCursor из предыдущего ответа API. Например: "?cursor=638712864000000000.42"
        // До точки — дата последнего показанного комментария, после точки — его Id (42)
        string[] parts = cursor.Split('.', 2);

        if (parts.Length == 2
            && long.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out long utcTicks)
            && long.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out long id)
            && utcTicks <= DateTimeOffset.MaxValue.UtcTicks
            && id > 0)
        {
            // Восстанавливаем дату и Id, по которым репозиторий найдёт следующие ответы.
            return new CommentReplyCursor(new DateTimeOffset(utcTicks, TimeSpan.Zero), id);
        }

        throw new ValidationException(new[]
        {
            new ValidationFailure(nameof(cursor), "Use the nextCursor value from the previous response.")
        });
    }


}
