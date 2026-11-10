using MessageBoard.Application.Comments;
using MessageBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessageBoard.Infrastructure.Persistence.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly MessageBoardDbContext _dbContext;

    public CommentRepository(MessageBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // 
    public void Add(Comment comment)
    {
        _dbContext.Comments.Add(comment);
    }

    public Task<bool> ExistsByIdAsync(long id, CancellationToken ct = default)
    {
        return _dbContext.Comments.AnyAsync(comment => comment.Id == id, ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _dbContext.SaveChangesAsync(ct);
    }

}