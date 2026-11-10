using MessageBoard.Domain.Entities;

namespace MessageBoard.Application.Comments;

public interface ICommentRepository
{
    void Add(Comment comment);

    Task<bool> ExistsByIdAsync(long id, CancellationToken ct = default);

    Task<int> SaveChangesAsync(CancellationToken ct = default);


}