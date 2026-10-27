namespace Comments.Application.Attachments;

public record FileUpload(
    Stream Content,
    string FileName
);