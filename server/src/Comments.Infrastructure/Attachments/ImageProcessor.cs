using Comments.Application.Attachments;

using ImageMagick;

namespace Comments.Infrastructure.Attachments;

public class ImageProcessor : IImageProcessor
{
    private const int MaxWidth = 320;
    private const int MaxHeight = 240;

    public ProcessedImage Process(Stream content, ImageFileFormat format)
    {
        MagickFormat magickFormat = ToMagickFormat(format);
        MagickReadSettings  settings = new() { Format = magickFormat };
        MagickGeometry geometry = new(MaxWidth, MaxHeight) { Greater = true };

        using  MagickImageCollection frames = new(content, settings);
        frames.Coalesce();

        foreach (IMagickImage<byte>? frame in frames)
        {
            frame.Resize(geometry);
        }

        return new ProcessedImage(
            Content: frames.ToByteArray(magickFormat),
            Width: (int)frames[0].Width,
            Height: (int)frames[0].Height
        );
    }

    private static MagickFormat ToMagickFormat(ImageFileFormat format) => format switch
    {
        ImageFileFormat.Jpeg => MagickFormat.Jpeg,
        ImageFileFormat.Png => MagickFormat.Png,
        ImageFileFormat.Gif => MagickFormat.Gif,
        _ => throw new ArgumentOutOfRangeException(nameof(format)),
    };
}