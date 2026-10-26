namespace Comments.Application.Captcha;

public interface ICaptchaService
{
    CaptchaDto Create();
    bool Validate(Guid id, string code);
}