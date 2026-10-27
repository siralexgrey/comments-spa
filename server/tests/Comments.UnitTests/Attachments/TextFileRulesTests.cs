using Comments.Application.Attachments;

namespace Comments.UnitTests.Attachments;

public class TextFileRulesTests
{
    [Fact]
    public void IsValid_AsciiTextFile_ReturnsTrue()
    {
        byte[] ascii = "hello"u8.ToArray();

        bool result = TextFileRules.IsValid(ascii);

        Assert.True(result);
    }

    [Fact]
    public void IsValid_CyrillicTextFile_ReturnsTrue()
    {
        byte[] cyrillic = "Привіт"u8.ToArray();

        bool result = TextFileRules.IsValid(cyrillic);

        Assert.True(result);
    }

    [Fact]
    public void IsValid_ExactlyMaxSizeFile_ReturnsTrue()
    {
        byte[] maxSize = new byte[TextFileRules.MaxSizeBytes];
        Array.Fill(maxSize, (byte)'a');

        bool result = TextFileRules.IsValid(maxSize);

        Assert.True(result);
    }

    [Fact]
    public void IsValid_OverMaxSizeFile_ReturnsFalse()
    {
        byte[] tooBig = new byte[TextFileRules.MaxSizeBytes + 1];
        Array.Fill(tooBig, (byte)'a');

        bool result = TextFileRules.IsValid(tooBig);

        Assert.False(result);
    }

    [Theory]
    [InlineData(new byte[] { 0x68, 0xFF, 0x69 })]   // invalid UTF-8
    [InlineData(new byte[] { 0x68, 0x00, 0x69 })]   // null byte
    [InlineData(new byte[] { })]                    // empty
    public void IsValid_InvalidContent_ReturnsFalse(byte[] content)
    {
        bool result = TextFileRules.IsValid(content);

        Assert.False(result);
    }
}