using MessageBoard.Application.Comments.Queries;

namespace MessageBoard.Application.Comments.DTOs;



public sealed record GetCommentsRequestDto
{
    public int PageNumber { get; init; } = 1;

    public CommentSortField SortBy { get; init; } = CommentSortField.CreatedAtUtc;

    public bool Descending { get; init; } = true;

}