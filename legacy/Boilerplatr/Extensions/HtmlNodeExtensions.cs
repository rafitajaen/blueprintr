using HtmlAgilityPack;

namespace Boilerplatr.Extensions;

public static class HtmlNodeExtensions
{
    public static string GetInnerText(this HtmlNode? node, string xpath)
    {
        return node?.SelectSingleNode(xpath).InnerText.Trim();
    }

    public static async Task<string?> GetImgBase64Async(this HtmlNode? node, string xpath, HttpClient client, CancellationToken cancellationToken = default)
    {
        string? base64 = null;

        var image = node?.SelectSingleNode(xpath);
        var src = image?.GetAttributeValue("src", string.Empty);

        if (!string.IsNullOrWhiteSpace(src))
        {
            // Si el src ya es un data URL
            if (src.StartsWith("data:image"))
            {
                // Extraer Base64 de la data URL
                base64 = src.Substring(src.IndexOf(",") + 1);
            }
            else
            {
                // Descargar la imagen usando HttpClient
                var bytes = await client.GetByteArrayAsync(src, cancellationToken);
                base64 = Convert.ToBase64String(bytes);
            }
        }

        return base64;
    }


}
