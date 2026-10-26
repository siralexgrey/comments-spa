using Comments.Application.Captcha;

using ImageMagick;
using ImageMagick.Drawing;

namespace Comments.Infrastructure.Captcha;

public class CaptchaImageGenerator : ICaptchaImageGenerator
{
    private static readonly string FontPath = Path.Combine(AppContext.BaseDirectory, "Captcha", "Fonts", "Roboto-Bold.ttf");
    private const int Width = 160;
    private const int Height = 60;

    public byte[] Generate(string code)
    {
        using var image = new MagickImage(MagickColors.White, Width, Height);

        new Drawables()
            .Font(FontPath)
            .FontPointSize(24)
            .FillColor(MagickColors.Black)
            .Text(15, 42, code)
            .Draw(image);

        image.Format = MagickFormat.Png;
        return image.ToByteArray();
    }
}