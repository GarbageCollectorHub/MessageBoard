namespace MessageBoard.Application.Comments.DTOs;




public sealed record CommentRepliesResponseDto(
        IReadOnlyList<CommentResponseDto> Items,

        // Для «Показать ещё» клиент передаёт курсор целиком, например ?cursor=638712864000000000.42.
        // Первое число — время создания (DateTimeOffset.UtcTicks) последнего комментария из Items, второе — его Id.
        // null означает, что больше ответов нет.
        string? NextCursor

    );