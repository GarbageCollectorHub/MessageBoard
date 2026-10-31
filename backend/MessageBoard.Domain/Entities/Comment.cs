namespace MessageBoard.Domain.Entities;

public class Comment
{
    public long Id { get; set; }

    public required string UserName { get; set; }

    public required string Email { get; set; }

    public string? HomePage { get; set; }

    public required string Text { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;


    // Root comments have no parent.
    public long? ParentId { get; set; }

    public Comment? Parent { get; set; }

    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}