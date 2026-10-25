using Comments.Application.Comments.Dtos;
using Comments.Application.Common;
using Comments.Domain.Entities;

using FluentValidation;

namespace Comments.Application.Comments.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .Matches("^[a-zA-Z0-9]+$")
            .WithMessage("User name should contain only latin letters and numbers")
            .MaximumLength(Comment.UserNameMaxLength)
            .WithMessage($"User name must be at most {Comment.UserNameMaxLength} characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Comment.EmailMaxLength);

        RuleFor(x => x.HomePage)
            .MaximumLength(Comment.HomePageMaxLength)
            .Must(UrlRules.IsHttpUrl)
            .WithMessage("User home page must be valid address starts with http:// or https://")
            .When(x => !string.IsNullOrEmpty(x.HomePage));

        RuleFor(x => x.Text)
            .NotEmpty();
    }
}