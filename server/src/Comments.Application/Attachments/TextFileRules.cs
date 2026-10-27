using System.Text.Unicode;

namespace Comments.Application.Attachments;

public static class TextFileRules
{
    public const int MaxSizeBytes = 100 * 1024;

    public static bool IsValid(ReadOnlySpan<byte> content) => !content.IsEmpty
        && content.Length <= MaxSizeBytes
        && Utf8.IsValid(content)
        && !content.Contains((byte)0);
}