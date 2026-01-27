using FileSignatures;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Boilerplatr.Utils;

public static partial class FileSignatureValidator
{
    public static bool TryGetByteArrayFromBase64(string data, [NotNullWhen(true)] out byte[]? file)
    {
        file = null;

        var base64 = Base64Regex().Replace(data, string.Empty);

        if (!Base64Regex().IsMatch(base64))
        {
            file = Convert.FromBase64String(base64);
        }

        return file is not null;
    }

    public static bool TryGetMimeType(this IFormFile file, out string? mimeType)
    {
        mimeType = null;

        try
        {
            using Stream stream = file.OpenReadStream();
            if (new FileFormatInspector().DetermineFileFormat(stream) is FileFormat format)
            {
                mimeType = format.MediaType;
            }
        }
        catch { /* This excepcion must be ignored because is a TryGet Method. */ }

        return !string.IsNullOrWhiteSpace(mimeType);
    }

    public static bool TryGetMimeType(this byte[] file, out string? mimeType)
    {
        mimeType = null;

        try
        {
            using Stream stream = new MemoryStream(file);
            if (new FileFormatInspector().DetermineFileFormat(stream) is FileFormat format)
            {
                mimeType = format.MediaType;
            }
        }
        catch { /* This excepcion must be ignored because is a TryGet Method. */ }

        return !string.IsNullOrWhiteSpace(mimeType);
    }

    public static bool IsPdf(this IFormFile file)
    {
        try
        {
            using Stream stream = file.OpenReadStream();
            return new FileFormatInspector().DetermineFileFormat(stream) is FileSignatures.Formats.Pdf;
        }
        catch { /* This excepcion must be ignored because is a TryGet Method. */ }

        return false;
    }

    public static bool IsPdf(this byte[] file)
    {
        try
        {
            using Stream stream = new MemoryStream(file);
            return new FileFormatInspector().DetermineFileFormat(stream) is FileSignatures.Formats.Pdf;
        }
        catch { /* This excepcion must be ignored because is a TryGet Method. */ }

        return false;
    }

    [GeneratedRegex(@"^data:([a-zA-Z0-9]+\/[a-zA-Z0-9-]+)?;base64,")]
    private static partial Regex Base64Regex();
}
