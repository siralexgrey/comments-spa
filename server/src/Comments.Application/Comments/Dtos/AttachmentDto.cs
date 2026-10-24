using Comments.Domain.Enums;

namespace Comments.Application.Comments.Dtos;

public record AttachmentDto(
    int Id,
    string OriginalName,
    AttachmentType Type,
    string Url,
    int? Width,
    int? Height
);