namespace Comments.Application.Attachments;

public static class ImageSignature
{
    private static readonly (byte[] Signature, ImageFileFormat Format)[] Signatures =
    [
        ([0xFF, 0xD8, 0xFF], ImageFileFormat.Jpeg),
        ([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], ImageFileFormat.Png),
        ("GIF87a"u8.ToArray(), ImageFileFormat.Gif),
        ("GIF89a"u8.ToArray(), ImageFileFormat.Gif),
    ];

    public static ImageFileFormat? Detect(ReadOnlySpan<byte> header)
    {
        foreach (var (signature, format) in Signatures)
        {
            if (header.StartsWith(signature))
            {
                return format;
            }
        }
        return null;
    }
}