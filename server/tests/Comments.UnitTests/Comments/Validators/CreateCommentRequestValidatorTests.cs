using Comments.Application.Captcha;
using Comments.Application.Comments.Dtos;
using Comments.Application.Comments.Validators;
using Comments.Domain.Entities;

using FluentValidation.TestHelper;

namespace Comments.UnitTests.Comments.Validators;

public class CreateCommentRequestValidatorTests
{
    private static readonly CreateCommentRequest ValidRequest = new(
        null,
        "ValidName",
        "validname@validmail.com",
        "https://valid.com",
        "Some <strong>text</strong>",
        "1:1:1:1",
        "example",
        Guid.NewGuid(),
        "aB3kZ"
    );
    private readonly CreateCommentRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidFormat_NoError()
    {
        var result = _validator.TestValidate(ValidRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("Кирилиця")]
    [InlineData("Name with space")]
    [InlineData("NameWith$Symbol")]
    public void UserName_InvalidFormat_HasError(string userName)
    {
        var result = _validator.TestValidate(ValidRequest with { UserName = userName });

        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Fact]
    public void UserName_ExceedMaxLength_HasError()
    {
        string longUserName = new('a', Comment.UserNameMaxLength + 1);

        var result = _validator.TestValidate(ValidRequest with { UserName = longUserName });

        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Fact]
    public void Email_InvalidFormat_HasError()
    {
        string email = "wrongformat.email";

        var result = _validator.TestValidate(ValidRequest with { Email = email });

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void HomePage_Null_NoError()
    {
        var result = _validator.TestValidate(ValidRequest with { HomePage = null });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("ftp://x.com")]
    [InlineData("not-a-url")]
    public void HomePage_InvalidFormat_HasError(string homePage)
    {
        var result = _validator.TestValidate(ValidRequest with { HomePage = homePage });

        result.ShouldHaveValidationErrorFor(x => x.HomePage);
    }

    [Fact]
    public void Text_Empty_HasError()
    {
        var result = _validator.TestValidate(ValidRequest with { Text = "" });

        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void CaptchaId_Empty_HasError()
    {
        var result = _validator.TestValidate(ValidRequest with { CaptchaId = Guid.Empty });

        result.ShouldHaveValidationErrorFor(x => x.CaptchaId);
    }

    [Fact]
    public void CaptchaCode_Empty_HasError()
    {
        var result = _validator.TestValidate(ValidRequest with { CaptchaCode = "" });

        result.ShouldHaveValidationErrorFor(x => x.CaptchaCode);
    }

    [Theory]
    [InlineData("ab-12")]
    [InlineData("abc")]
    public void CaptchaCode_InvalidFormat_HasError(string code)
    {
        var result = _validator.TestValidate(ValidRequest with { CaptchaCode = code });

        result.ShouldHaveValidationErrorFor(x => x.CaptchaCode);
    }
}