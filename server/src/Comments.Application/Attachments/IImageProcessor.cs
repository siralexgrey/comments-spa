namespace Comments.Application.Attachments;

public interface IImageProcessor
{
    ProcessedImage Process(Stream content, ImageFileFormat format);
}