using Comments.Application.Attachments;

namespace Comments.UnitTests.Attachments;

public class ImageSignatureTests
{
    [Theory]
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, ImageFileFormat.Jpeg)]
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00 }, ImageFileFormat.Png)]
    [InlineData(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61, 0x00, 0x00 }, ImageFileFormat.Gif)]
    [InlineData(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x00, 0x00 }, ImageFileFormat.Gif)]
    public void Detect_ValidBytes_ReturnsExpectedFormat(byte[] header, ImageFileFormat expected)
    {
        var format = ImageSignature.Detect(header);

        Assert.Equal(expected, format);
    }

    [Theory]
    [InlineData(new byte[] { 0x28, 0x33 })] // trash
    [InlineData(new byte[0])] // empty
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D })] // truncated png
    [InlineData(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x38, 0x61 })] // GIF88a
    public void Detect_InvalidBytes_ReturnsNull(byte[] header)
    {
        var format = ImageSignature.Detect(header);

        Assert.Null(format);
    }
}