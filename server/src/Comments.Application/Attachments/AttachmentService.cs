using Comments.Application.Comments.Dtos;
using Comments.Domain.Entities;
using Comments.Domain.Enums;

using FluentValidation;
using FluentValidation.Results;

namespace Comments.Application.Attachments;

public class AttachmentService(
    IImageProcessor processor,
    IFileStorage storage
)
{
    private readonly IImageProcessor _processor = processor;
    private readonly IFileStorage _storage = storage;

    public async Task<Attachment> SaveAsync(FileUpload upload, CancellationToken cancellationToken)
    {
        using var buffer = new MemoryStream();
        await upload.Content.CopyToAsync(buffer, cancellationToken);
        byte[] content = buffer.ToArray();
        string truncatedFileName = upload.FileName.Length > Attachment.OriginalNameMaxLength
            ? upload.FileName[..Attachment.OriginalNameMaxLength]
            : upload.FileName;

        if (ImageSignature.Detect(content) is { } format)
        {
            return await SaveImageAsync(content, format, truncatedFileName, cancellationToken);
        }
        if (string.Equals(Path.GetExtension(upload.FileName), ".txt", StringComparison.OrdinalIgnoreCase))
        {
            return await SaveTextAsync(content, truncatedFileName, cancellationToken);
        }
        throw InvalidAttachment("Unsupported file type. Allowed: JPG, PNG, GIF, TXT.");
    }

    private async Task<Attachment> SaveImageAsync(
        byte[] content,
        ImageFileFormat format,
        string fileName,
        CancellationToken cancellationToken
    )
    {
        (string extension, string contentType) = ToFileInfo(format);
        using var input = new MemoryStream(content);
        var image = _processor.Process(input, format) ?? throw InvalidAttachment("Image file is corrupted.");
        var storedName = await _storage.SaveAsync(image.Content, extension, cancellationToken);
        return new Attachment(
            type: AttachmentType.Image,
            originalName: fileName,
            storedName: storedName,
            contentType: contentType,
            sizeBytes: image.Content.Length,
            width: image.Width,
            height: image.Height
        );
    }

    private async Task<Attachment> SaveTextAsync(byte[] content, string fileName, CancellationToken cancellationToken)
    {
        if (!TextFileRules.IsValid(content)) throw InvalidAttachment("Text file must be UTF-8 and not larger than 100 KB.");
        var storedName = await _storage.SaveAsync(content, ".txt", cancellationToken);
        return new Attachment(
            type: AttachmentType.Text,
            originalName: fileName,
            storedName: storedName,
            contentType: "text/plain",
            sizeBytes: content.Length,
            width: null,
            height: null
        );
    }

    private static (string Extension, string ContentType) ToFileInfo(ImageFileFormat format) => format switch
    {
        ImageFileFormat.Jpeg => (".jpg", "image/jpeg"),
        ImageFileFormat.Png => (".png", "image/png"),
        ImageFileFormat.Gif => (".gif", "image/gif"),
        _ => throw new ArgumentOutOfRangeException(nameof(format)),
    };

    private static ValidationException InvalidAttachment(string message) =>
        new([new ValidationFailure(nameof(CreateCommentRequest.Attachment), message)]);
}