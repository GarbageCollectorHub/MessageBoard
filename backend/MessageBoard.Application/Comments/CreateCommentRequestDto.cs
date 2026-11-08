namespace MessageBoard.Application.Comments;


public record CreateCommentRequestDto
{
    public string? UserName { get; init; }

    public string? Email { get; init; }

    public string? HomePage { get; init; }

    public string? Text { get; init; }

    public long? ParentId { get; init; }

}