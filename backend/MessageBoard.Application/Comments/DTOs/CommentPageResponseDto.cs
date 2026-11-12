namespace MessageBoard.Application.Comments.DTOs;

public sealed record CommentPageResponseDto(
            IReadOnlyList<CommentResponseDto> Items,
            int PageNumber,
            int PageSize,
            long TotalCount
            )
{
    public long TotalPages
    {
        get
        {
            var pages = TotalCount / PageSize;

            if (TotalCount % PageSize > 0)
            {
                pages++;
            }

            return pages;
        }
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

}