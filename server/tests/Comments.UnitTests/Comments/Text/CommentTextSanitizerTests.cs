using Comments.Application.Comments.Text;

namespace Comments.UnitTests.Comments.Text;

public class CommentTextSanitizerTests
{
    private readonly CommentTextSanitizer _sanitizer = new();

    [Fact]
    public void Sanitize_PlainText_ReturnsSameText()
    {
        var result = _sanitizer.Sanitize("hello");

        Assert.True(result.IsValid);
        Assert.Equal("hello", result.Html);
    }

    [Fact]
    public void Sanitize_UnclosedTah_ReturnsError()
    {
        var result = _sanitizer.Sanitize("<i>hello");

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("<strong>hi</strong>")]
    [InlineData("a &lt; b")]
    public void Sanitize_ValidHtml_ReturnsSameHtml(string text)
    {
        var result = _sanitizer.Sanitize(text);

        Assert.True(result.IsValid);
        Assert.Equal(text, result.Html);
    }

    [Theory]
    [InlineData("<b>hi</hi>")]
    [InlineData("<script>alert(1)</script>")]
    public void Sanitize_InvalidHtmlTag_ReturnsError(string text)
    {
        var result = _sanitizer.Sanitize(text);

        Assert.False(result.IsValid);
    }
}