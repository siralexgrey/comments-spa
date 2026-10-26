using Comments.Application.Captcha;

using ImageMagick;
using ImageMagick.Drawing;

namespace Comments.Infrastructure.Captcha;

public class CaptchaImageGenerator : ICaptchaImageGenerator
{
    private static readonly string FontPath = Path.Combine(AppContext.BaseDirectory, "Captcha", "Fonts", "Roboto-Bold.ttf");
    private const int Width = 160;
    private const int Height = 60;
    private const int LineCount = 4;
    private const double StrokeWidth = 1.5;
    private const int FontSize = 32;
    private const int Padding = 15;
    private const int MaxJitter = 4;
    private const int WaveAmplitude = 3;
    private const int WaveLength = 40;
    private static readonly MagickColor TextColor = MagickColors.Black;
    private static readonly MagickColor LineColor = MagickColors.Gray;

    public byte[] Generate(string code)
    {
        using var image = new MagickImage(MagickColors.White, Width, Height);
        image.Settings.Font = FontPath;
        image.Settings.FontPointsize = FontSize;
        ITypeMetric metrics = image.FontTypeMetrics(code)
            ?? throw new InvalidOperationException("Failed to measure captcha text.");

        double baseline = (Height - metrics.TextHeight) / 2 + metrics.Ascent;

        Drawables drawables = new();

        drawables
            .Font(FontPath)
            .FontPointSize(FontSize)
            .FillColor(TextColor);

        int step = (Width - 2 * Padding) / code.Length;
        for (int i = 0; i < code.Length; i++)
        {
            double y = baseline + Random.Shared.Next(-MaxJitter, MaxJitter + 1);
            int x = Padding + i * step;

            drawables.Text(x, y, code[i].ToString());
        }

        drawables
            .StrokeColor(LineColor)
            .StrokeWidth(StrokeWidth);

        for (int i = 0; i < LineCount; i++)
        {
            int startX = Random.Shared.Next(Width);
            int startY = Random.Shared.Next(Height);
            int endX = Random.Shared.Next(Width);
            int endY = Random.Shared.Next(Height);

            drawables
                .Line(startX, startY, endX, endY);
        }

        drawables.Draw(image);

        image.Wave(PixelInterpolateMethod.Bilinear, WaveAmplitude, WaveLength);
        image.AddNoise(NoiseType.Gaussian);

        image.Format = MagickFormat.Png;
        return image.ToByteArray();
    }
}