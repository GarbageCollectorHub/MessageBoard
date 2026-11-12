namespace MessageBoard.Application.Comments.Queries;



public static class CommentPagination
{
    public const int PageSize = 25;

    // Skip принимает int, поэтому смещение (PageNumber - 1) * PageSize не должно превышать int.MaxValue.
    public const int MaxPageNumber = int.MaxValue / PageSize + 1;

}