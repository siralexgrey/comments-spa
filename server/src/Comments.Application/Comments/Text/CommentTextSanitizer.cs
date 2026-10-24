using System.Net;
using System.Text;
using System.Xml;

namespace Comments.Application.Comments.Text;

public sealed class CommentTextSanitizer : ICommentTextSanitizer
{
    private static readonly HashSet<string> AllowedTags = ["a", "code", "i", "strong"];
    public SanitizeResult Sanitize(string text)
    {
        string xml = $"<root>{text}</root>";
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit };
        try
        {
            using var reader = XmlReader.Create(new StringReader(xml), settings);
            var sb = new StringBuilder();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Depth == 0) continue;
                        if (!AllowedTags.Contains(reader.Name))
                        {
                            return SanitizeResult.Failure($"Tag <{reader.Name}> is not allowed");
                        }
                        sb.Append($"<{reader.Name}>");
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Depth == 0) continue;
                        sb.Append($"</{reader.Name}>");
                        break;
                    case XmlNodeType.Text:
                        sb.Append(WebUtility.HtmlEncode(reader.Value));
                        break;
                }
            }
            return SanitizeResult.Success(sb.ToString());
        }
        catch (XmlException ex)
        {
            return SanitizeResult.Failure(ex.Message);
        }

    }
}