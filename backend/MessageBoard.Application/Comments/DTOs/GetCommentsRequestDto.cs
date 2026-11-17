using MessageBoard.Application.Comments.Queries;
using System.ComponentModel;

namespace MessageBoard.Application.Comments.DTOs;



public sealed record GetCommentsRequestDto
{
    [Description("Page number from 1 to 85899346. Defaults to 1.")]
    public int PageNumber { get; init; } = 1;

    [Description("Sort field. Use 0 for creation date, 1 for user name, or 2 for email. Defaults to 0.")]
    public CommentSortField SortBy { get; init; } = CommentSortField.CreatedAtUtc;

    [Description("Use true for descending order or false for ascending order. Defaults to true.")]
    public bool Descending { get; init; } = true;

}