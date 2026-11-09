namespace MessageBoard.Application.Comments.DTOs;



public sealed record CommentResponseDto(
        long    Id,
        string  UserName,
        string? HomePage,
        string  Text,
        long?   ParentId,
        DateTimeOffset CreatedAtUtc
    );