using Comments.Application.Captcha;

using Microsoft.Extensions.Caching.Memory;

namespace Comments.UnitTests.Captcha;

public class CaptchaServiceTests
{
    private readonly FakeCaptchaImageGenerator _generator = new();
    private readonly CaptchaService _service;

    public CaptchaServiceTests()
    {
        _service = new CaptchaService(
            new MemoryCache(new MemoryCacheOptions()),
            _generator
        );
    }

    [Fact]
    public void Validate_CorrectCode_ReturnsTrue()
    {
        CaptchaDto captcha = _service.Create();
        string code = _generator.LastCode!;

        Assert.True(_service.Validate(captcha.Id, code));
    }

    [Fact]
    public void Validate_CorrectCodeInDifferentCase_ReturnsTrue()
    {
        CaptchaDto captcha = _service.Create();
        string code = _generator.LastCode!.ToLowerInvariant();

        Assert.True(_service.Validate(captcha.Id, code));
    }

    [Fact]
    public void Validate_WrongCode_ReturnsFalse()
    {
        CaptchaDto captcha = _service.Create();
        string code = _generator.LastCode!;
        string wrongCode = WrongCodeFor(code);

        Assert.False(_service.Validate(captcha.Id, wrongCode));
    }

    [Fact]
    public void Validate_UnknownId_ReturnsFalse()
    {
        _service.Create();
        string code = _generator.LastCode!;

        Assert.False(_service.Validate(Guid.NewGuid(), code));
    }

    [Fact]
    public void Validate_SameIdTwice_ReturnsFalse()
    {
        CaptchaDto captcha = _service.Create();
        string code = _generator.LastCode!;
        _service.Validate(captcha.Id, code);

        Assert.False(_service.Validate(captcha.Id, code));
    }

    [Fact]
    public void Validate_CorrectCodeAfterWrongAttempt_ReturnsFalse()
    {
        CaptchaDto captcha = _service.Create();
        string code = _generator.LastCode!;
        string wrongCode = WrongCodeFor(code);
        _service.Validate(captcha.Id, wrongCode);

        Assert.False(_service.Validate(captcha.Id, code));
    }

    [Fact]
    public void Create_ReturnsPngDataUrl()
    {
        CaptchaDto captcha = _service.Create();
        Assert.StartsWith("data:image/png;base64,", captcha.ImageDataUrl);
    }

    private static string WrongCodeFor(string code)
    {
        return char.ToUpperInvariant(code[0]) == 'A'
            ? 'B' + code[1..]
            : 'A' + code[1..];
    }

    private sealed class FakeCaptchaImageGenerator : ICaptchaImageGenerator
    {
        public string? LastCode { get; private set; }

        public byte[] Generate(string code)
        {
            LastCode = code;
            return [1, 2, 3];
        }
    }
}