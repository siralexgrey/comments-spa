using Comments.Application.Comments.Text;

namespace Comments.UnitTests.Comments.Text;

public class CommentTextSanitizerTests
{
    private readonly CommentTextSanitizer _sanitizer = new();

    [Theory]
    [InlineData("hello", "hello")]
    [InlineData("<strong>hi</strong>", "<strong>hi</strong>")]
    [InlineData("a &lt; b", "a &lt; b")]
    [InlineData("<i>a</i> <i>b</i>", "<i>a</i> <i>b</i>")]
    [InlineData("a<i/>b", "a<i></i>b")]
    [InlineData("<a href=\"https://test.com\" title=\"testTitle\">test</a>", "<a href=\"https://test.com\" title=\"testTitle\">test</a>")]
    [InlineData("<a href=\"https://test.com\" title=\"testTitle\"/>", "<a href=\"https://test.com\" title=\"testTitle\"></a>")]
    public void Sanitize_ValidHtml_ReturnsExpectedHtml(string text, string expected)
    {
        var result = _sanitizer.Sanitize(text);

        Assert.True(result.IsValid);
        Assert.Equal(expected, result.Html);
    }

    [Fact]
    public void Sanitize_UnclosedTah_ReturnsError()
    {
        var result = _sanitizer.Sanitize("<i>hello");

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("<b>hi</hi>")]
    [InlineData("<script>alert(1)</script>")]
    public void Sanitize_InvalidHtmlTag_ReturnsError(string text)
    {
        var result = _sanitizer.Sanitize(text);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("<i title=\"t\">test</i>")]
    [InlineData("<a onclick=\"alert(1)\">test</a>")]
    public void Sanitize_InvalidAttribute_ReturnsError(string text)
    {
        var result = _sanitizer.Sanitize(text);

        Assert.False(result.IsValid);
    }
}