using MessageBoard.Application.Comments;
using MessageBoard.Application.Comments.DTOs;
using MessageBoard.Application.Comments.Queries;
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


    public async Task<CommentPageResponseDto> GetRootPageAsync(GetCommentsRequestDto request, CancellationToken ct = default)
    {
        var query = _dbContext.Comments
            .AsNoTracking()
            .Where(comment => comment.ParentId == null);

        long totalCount = await query.LongCountAsync(ct);

        var orderedQuery = (request.SortBy, request.Descending) switch
        {
            (CommentSortField.UserName, false) => query.OrderBy(comment => comment.UserName),
            (CommentSortField.UserName, true) => query.OrderByDescending(comment => comment.UserName),

            (CommentSortField.Email, false) => query.OrderBy(comment => comment.Email),
            (CommentSortField.Email, true) => query.OrderByDescending(comment => comment.Email),

            (CommentSortField.CreatedAtUtc, false) => query.OrderBy(comment => comment.CreatedAtUtc),
            (CommentSortField.CreatedAtUtc, true) => query.OrderByDescending(comment => comment.CreatedAtUtc),

            _ => throw new ArgumentOutOfRangeException(nameof(request.SortBy))
        };

        // Если значения выбранного поля совпали, сортируем эти комментарии между собой по Id.
        var sortedQuery = request.Descending
            ? orderedQuery.ThenByDescending(comment => comment.Id)
            : orderedQuery.ThenBy(comment => comment.Id);

        int skip = (request.PageNumber - 1) * CommentPagination.PageSize;

        var items = await sortedQuery
            .Skip(skip)
            .Take(CommentPagination.PageSize)
            .Select(comment => new CommentResponseDto(
                comment.Id,
                comment.UserName,
                comment.HomePage,
                comment.Text,
                comment.ParentId,
                comment.CreatedAtUtc,
                comment.Replies.Count()))
            .ToListAsync(ct);

        return new CommentPageResponseDto(items, request.PageNumber, CommentPagination.PageSize, totalCount);
    }

}