namespace Comments.Application.Comments.Text;

public sealed record SanitizeResult(string? Html, string? Error)
{
    public bool IsValid => Error is null;

    public static SanitizeResult Success(string html) => new(html, null);
    public static SanitizeResult Failure(string error) => new(null, error);
}