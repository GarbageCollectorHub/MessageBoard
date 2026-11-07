using MessageBoard.Domain.Entities;

namespace MessageBoard.Application.Comments;

public interface ICommentRepository
{
    void Add(Comment comment);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


}