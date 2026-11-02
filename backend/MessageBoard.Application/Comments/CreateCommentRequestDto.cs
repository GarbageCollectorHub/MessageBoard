namespace MessageBoard.Application.Comments;

public class CreateCommentRequestDto
{
    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? HomePage { get; set; }

    public string? Text { get; set; }
}