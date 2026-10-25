using System.Net;
using System.Text;
using System.Xml;

using Comments.Application.Common;

namespace Comments.Application.Comments.Text;

public sealed class CommentTextSanitizer : ICommentTextSanitizer
{
    private static readonly HashSet<string> AllowedTags = ["a", "code", "i", "strong"];
    private static readonly HashSet<string> AllowedLinkAttributes = ["href", "title"];
    private const string RootOpenTag = "<root>";

    public SanitizeResult Sanitize(string text)
    {
        string xml = $"{RootOpenTag}{text}</root>";
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
                        string tagName = reader.Name;
                        if (!AllowedTags.Contains(tagName))
                        {
                            return SanitizeResult.Failure($"Tag <{tagName}> is not allowed");
                        }
                        sb.Append($"<{tagName}");
                        while (reader.MoveToNextAttribute())
                        {
                            if (tagName != "a" || !AllowedLinkAttributes.Contains(reader.Name))
                            {
                                return SanitizeResult.Failure($"Attribute {reader.Name} is not allowed on <{tagName}>");
                            }
                            if (reader.Name == "href" && !UrlRules.IsHttpUrl(reader.Value))
                            {
                                return SanitizeResult.Failure("Link href must be an http or https URL");
                            }
                            sb.Append($" {reader.Name}=\"{WebUtility.HtmlEncode(reader.Value)}\"");
                        }
                        reader.MoveToElement();
                        sb.Append('>');
                        if (reader.IsEmptyElement)
                        {
                            sb.Append($"</{tagName}>");
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Depth == 0) continue;
                        sb.Append($"</{reader.Name}>");
                        break;
                    case XmlNodeType.Text:
                    case XmlNodeType.Whitespace:
                        sb.Append(WebUtility.HtmlEncode(reader.Value));
                        break;
                }
            }
            return SanitizeResult.Success(sb.ToString());
        }
        catch (XmlException ex)
        {
            int position = ex.LineNumber == 1 ? ex.LinePosition - RootOpenTag.Length : ex.LinePosition;
            return SanitizeResult.Failure($"Invalid markup or unclosed tag at line {ex.LineNumber}, position {position}.");
        }
    }
}