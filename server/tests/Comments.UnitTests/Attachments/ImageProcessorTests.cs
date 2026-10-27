using Comments.Application.Attachments;
using Comments.Infrastructure.Attachments;

using ImageMagick;

namespace Comments.UnitTests.Attachments;

public class ImageProcessorTests
{
    public static TheoryData<
        ImageFileFormat,
        (int Width, int Height),
        (int Width, int Height)
    > Cases => new()
    {
        {ImageFileFormat.Jpeg, (1000, 500), (320, 160)},
        {ImageFileFormat.Png, (300, 900), (80, 240)},
        {ImageFileFormat.Png, (100, 80), (100, 80)}
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void Process_ValidImage_ReturnsExpected(
        ImageFileFormat format,
        (int Width, int Height) size,
        (int Width, int Height) expected)
    {
        var magickFormat = ToMagickFormat(format);
        using var source = new MagickImage(MagickColors.Red, (uint)size.Width, (uint)size.Height);
        using var input = new MemoryStream(source.ToByteArray(magickFormat));

        var result = new ImageProcessor().Process(input, format);
        var resultInfo = new MagickImageInfo(result.Content);

        Assert.Equal(magickFormat, resultInfo.Format);
        Assert.Equal(expected.Width, (int)resultInfo.Width);
        Assert.Equal(expected.Width, result.Width);
        Assert.Equal(expected.Height, (int)resultInfo.Height);
        Assert.Equal(expected.Height, result.Height);
    }

    [Fact]
    public void Process_AnimatedGif_ResizesAllFrames()
    {
        using var source = new MagickImageCollection();
        source.Add(new MagickImage(MagickColors.Red, 640, 480));
        var patch = new MagickImage(MagickColors.Green, 100, 100)
        {
            Page = new MagickGeometry(50, 50, 100, 100)
        };
        source.Add(patch);
        source.Add(new MagickImage(MagickColors.Blue, 640, 480));
        using var input = new MemoryStream(source.ToByteArray(MagickFormat.Gif));

        var result = new ImageProcessor().Process(input, ImageFileFormat.Gif);
        using var resultFrames = new MagickImageCollection(result.Content);

        Assert.Equal(3, resultFrames.Count);
        Assert.All(resultFrames, frame =>
        {
            Assert.Equal(320u, frame.Width);
            Assert.Equal(240u, frame.Height);
        });
    }

    private static MagickFormat ToMagickFormat(ImageFileFormat format) => format switch
    {
        ImageFileFormat.Jpeg => MagickFormat.Jpeg,
        ImageFileFormat.Png => MagickFormat.Png,
        ImageFileFormat.Gif => MagickFormat.Gif,
        _ => throw new ArgumentOutOfRangeException(nameof(format)),
    };
}