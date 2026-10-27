namespace Comments.Application.Attachments;

public record ProcessedImage(
    byte[] Content,
    int Width,
    int Height
);