using System.Text;
using System.Text.RegularExpressions;
using Markdig;

namespace Boilerplatr.Utils;

public static class Markdowner
{
    public static string ToHtml(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (Match line in StringUtilities.LineRegex().Matches(input))
        {
            sb.AppendLine(Markdown.ToHtml(line.Value.Trim()));
        }

        return sb.ToString();
    }

    public static string ToHtmlList(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (Match line in StringUtilities.LineRegex().Matches(input))
        {
            if (!string.IsNullOrWhiteSpace(line.Value))
            {
                sb.AppendLine($"- {line.Value.Trim()}");
            }
        }

        return Markdown.ToHtml(sb.ToString());
    }
}
