namespace Comments.Application.Captcha;

public record CaptchaDto(
    Guid Id,
    string ImageDataUrl
);