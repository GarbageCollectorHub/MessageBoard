using MessageBoard.Application.Comments.DTOs;
using MessageBoard.Application.Comments.Queries;
using MessageBoard.Domain.Entities;

namespace MessageBoard.Application.Comments;

public interface ICommentRepository
{
    void Add(Comment comment);

    Task<bool> ExistsByIdAsync(long id, CancellationToken ct = default);

    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task<CommentPageResponseDto> GetRootPageAsync(GetCommentsRequestDto request, 
        CancellationToken ct = default);

    // Возвращает до limit прямых replies для указанного Comment
    // При cursor = null выбирает replies с начала ветки
    Task<IReadOnlyList<CommentResponseDto>> GetRepliesAsync(
        long parentId, CommentReplyCursor? cursor, int limit, CancellationToken ct = default);

}