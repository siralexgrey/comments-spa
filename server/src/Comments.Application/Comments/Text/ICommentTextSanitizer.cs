namespace Comments.Application.Comments.Text;

public interface ICommentTextSanitizer
{
    SanitizeResult Sanitize(string text);
}