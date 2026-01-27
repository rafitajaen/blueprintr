using FileSignatures;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Blueprintr.Utils;

/// <summary>
/// Provides validation and inspection methods for file signatures and MIME types.
/// </summary>
/// <remarks>
/// This class uses the FileSignatures library to determine file formats based on their binary signatures
/// rather than relying on file extensions. It provides methods for MIME type detection, PDF validation,
/// and Base64 data conversion.
/// Added in version 1.0.0.
/// </remarks>
public static partial class FileSignatureValidator
{
    /// <summary>
    /// Attempts to convert a Base64-encoded string to a byte array, removing any data URI prefix.
    /// </summary>
    /// <param name="data">The Base64-encoded string, optionally with a data URI prefix (e.g., "data:image/png;base64,...").</param>
    /// <param name="file">
    /// When this method returns, contains the decoded byte array if the conversion succeeded,
    /// or <c>null</c> if the conversion failed.
    /// </param>
    /// <returns><c>true</c> if the conversion succeeded; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method automatically strips data URI prefixes (e.g., "data:image/png;base64,") before decoding.
    /// Added in version 1.0.0.
    /// </remarks>
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

    /// <summary>
    /// Attempts to determine the MIME type of an uploaded file by inspecting its binary signature.
    /// </summary>
    /// <param name="file">The uploaded file to inspect.</param>
    /// <param name="mimeType">
    /// When this method returns, contains the detected MIME type if successful,
    /// or <c>null</c> if the MIME type could not be determined.
    /// </param>
    /// <returns><c>true</c> if the MIME type was successfully determined; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method reads the file's binary signature to determine its true format,
    /// regardless of the file extension. Exceptions are silently caught and ignored.
    /// Added in version 1.0.0.
    /// </remarks>
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
        catch { /* This exception must be ignored because this is a TryGet method. */ }

        return !string.IsNullOrWhiteSpace(mimeType);
    }

    /// <summary>
    /// Attempts to determine the MIME type of a byte array by inspecting its binary signature.
    /// </summary>
    /// <param name="file">The byte array representing the file to inspect.</param>
    /// <param name="mimeType">
    /// When this method returns, contains the detected MIME type if successful,
    /// or <c>null</c> if the MIME type could not be determined.
    /// </param>
    /// <returns><c>true</c> if the MIME type was successfully determined; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method reads the binary signature to determine the file's true format.
    /// Exceptions are silently caught and ignored.
    /// Added in version 1.0.0.
    /// </remarks>
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
        catch { /* This exception must be ignored because this is a TryGet method. */ }

        return !string.IsNullOrWhiteSpace(mimeType);
    }

    /// <summary>
    /// Determines whether an uploaded file is a PDF by inspecting its binary signature.
    /// </summary>
    /// <param name="file">The uploaded file to inspect.</param>
    /// <returns><c>true</c> if the file is a PDF; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method validates the file format based on its binary signature, not the file extension.
    /// Exceptions are silently caught and the method returns <c>false</c>.
    /// Added in version 1.0.0.
    /// </remarks>
    public static bool IsPdf(this IFormFile file)
    {
        try
        {
            using Stream stream = file.OpenReadStream();
            return new FileFormatInspector().DetermineFileFormat(stream) is FileSignatures.Formats.Pdf;
        }
        catch { /* This exception must be ignored because this is a validation method. */ }

        return false;
    }

    /// <summary>
    /// Determines whether a byte array represents a PDF by inspecting its binary signature.
    /// </summary>
    /// <param name="file">The byte array representing the file to inspect.</param>
    /// <returns><c>true</c> if the file is a PDF; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method validates the file format based on its binary signature.
    /// Exceptions are silently caught and the method returns <c>false</c>.
    /// Added in version 1.0.0.
    /// </remarks>
    public static bool IsPdf(this byte[] file)
    {
        try
        {
            using Stream stream = new MemoryStream(file);
            return new FileFormatInspector().DetermineFileFormat(stream) is FileSignatures.Formats.Pdf;
        }
        catch { /* This exception must be ignored because this is a validation method. */ }

        return false;
    }

    /// <summary>
    /// Regular expression pattern for matching and removing data URI prefixes from Base64 strings.
    /// </summary>
    /// <returns>A compiled regular expression for matching data URI prefixes.</returns>
    /// <remarks>
    /// Matches patterns like "data:image/png;base64," or "data:;base64,".
    /// Added in version 1.0.0.
    /// </remarks>
    [GeneratedRegex(@"^data:([a-zA-Z0-9]+\/[a-zA-Z0-9-]+)?;base64,")]
    private static partial Regex Base64Regex();
}
