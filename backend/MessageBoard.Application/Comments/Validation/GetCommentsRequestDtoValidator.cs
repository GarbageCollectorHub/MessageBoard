using FluentValidation;
using MessageBoard.Application.Comments.DTOs;
using MessageBoard.Application.Comments.Queries;

namespace MessageBoard.Application.Comments.Validation;


public sealed class GetCommentsRequestDtoValidator : AbstractValidator<GetCommentsRequestDto>
{
    public GetCommentsRequestDtoValidator()
    {
        RuleFor(request => request.PageNumber)
            .InclusiveBetween(1, CommentPagination.MaxPageNumber)
            .WithMessage("Page number must be between {From} and {To}.");

        RuleFor(request => request.SortBy)
            .IsInEnum()
            .WithMessage("Sort by CreatedAtUtc, UserName, or Email.");
    }
}