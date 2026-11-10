using System.Net.Mail;
using FluentValidation;
using MessageBoard.Application.Comments.DTOs;
using MessageBoard.Domain.Constants;

namespace MessageBoard.Application.Comments.Validation;

public class CreateCommentRequestDtoValidator : AbstractValidator<CreateCommentRequestDto>
{

    public CreateCommentRequestDtoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // Имя может содержать только латинские буквы A-Z, a-z и цифры 0-9.
        RuleFor(request => request.UserName)
            .NotEmpty()
            .WithMessage("User name is required.")
            .MaximumLength(CommentLimits.UserNameMaxLength)
            .WithMessage("User name must be {MaxLength} characters or fewer.")
            .Must(IsValidUserName)
            .WithMessage("Use only Latin letters and digits.");

        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .MaximumLength(CommentLimits.EmailMaxLength)
            .WithMessage("Email must be {MaxLength} characters or fewer.")
            .EmailAddress()
            .WithMessage("Enter a valid email.")
            .Must(IsValidEmail)
            .WithMessage("Enter a valid email.");

        // Необязательное поле. Допускаются только абсолютные HTTP/HTTPS URL.
        RuleFor(request => request.HomePage)
            .MaximumLength(CommentLimits.HomePageMaxLength)
            .WithMessage("Home page must be {MaxLength} characters or fewer.")
            .Must(IsValidHomePage)
            .WithMessage("Enter a valid HTTP or HTTPS URL without a login or password.");

        // Обязательное поле. Пробелы не считаются содержимым.
        RuleFor(request => request.Text)
            .NotEmpty()
            .WithMessage("Text is required.")
            .MaximumLength(CommentLimits.TextMaxLength)
            .WithMessage("Text must be {MaxLength} characters or fewer.")
            .Must(CommentHtmlValidator.IsValid)
            .WithMessage("Text contains invalid or unsupported markup. Allowed tags: a, code, i, strong.");

        RuleFor(request => request.ParentId)
            .GreaterThan(0)
            .When(request => request.ParentId.HasValue)
            .WithMessage("Parent ID must be greater than zero.");
    }

    private static bool IsValidUserName(string? userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return false;
        }

        return userName.All(char.IsAsciiLetterOrDigit);
    }


    private static bool IsValidEmail(string? email)
    {
        if (!MailAddress.TryCreate(email, out var emailAddress))
        {
            return false;
        }

        return emailAddress.Address == email;
    }

    private static bool IsValidHomePage(string? homePage)
    {
        // Пустое значение допустимо для необязательного поля
        return string.IsNullOrEmpty(homePage) || HttpUrlValidator.IsValid(homePage);
    }

}