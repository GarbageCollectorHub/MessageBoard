namespace MessageBoard.Application.Comments.Exceptions;



public sealed class ParentCommentNotFoundException(long parentId)
        : Exception($"Parent comment with ID {parentId} was not found.");

