using MessageBoard.Application.Comments;
using MessageBoard.Domain.Entities;

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

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

}