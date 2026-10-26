namespace Comments.Application.Captcha;

public interface ICaptchaImageGenerator
{
    byte[] Generate(string code);
}