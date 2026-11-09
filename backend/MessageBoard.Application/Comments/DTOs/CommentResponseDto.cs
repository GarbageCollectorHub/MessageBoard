namespace MessageBoard.Application.Comments;



public sealed record CommentResponseDto(
        long    Id,
        string  UserName,
        string? HomePage,
        string  Text,
        long?   ParentId,
        DateTimeOffset CreatedAtUtc
    );