

namespace MessageBoard.Application.Comments.Queries
{

    // Хранит дату и ID последнего Reply, возвращённого для конкретного Comment
    // Чтобы при "Show more" загрузка продолжалась со следующего Reply
    public sealed record CommentReplyCursor(DateTimeOffset CreatedAtUtc, long Id);

}
