using MessageBoard.Domain.Entities;

namespace MessageBoard.Application.Comments;

public interface ICommentRepository
{
    void Add(Comment comment);

    Task<bool> ExistsByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


}