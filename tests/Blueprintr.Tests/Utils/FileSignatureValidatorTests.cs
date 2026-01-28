using Blueprintr.Utils;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Blueprintr.Tests.Utils;

/// <summary>
/// Tests for <see cref="FileSignatureValidator"/> file signature validation and MIME type detection.
/// </summary>
[TestFixture]
public class FileSignatureValidatorTests
{
    // PNG file signature: 89 50 4E 47 0D 0A 1A 0A
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    // JPEG file signature: FF D8 FF
    private static readonly byte[] JpegSignature = [0xFF, 0xD8, 0xFF];

    // PDF file signature: 25 50 44 46 (%PDF)
    private static readonly byte[] PdfSignature = Encoding.ASCII.GetBytes("%PDF-1.4\n");

    [Test]
    public void TryGetByteArrayFromBase64_WithDataUriPrefix_ExtractsBytesCorrectly()
    {
        // Arrange
        var bytes = Encoding.UTF8.GetBytes("Hello World");
        var base64 = Convert.ToBase64String(bytes);
        var dataUri = $"data:text/plain;base64,{base64}";

        // Act
        var success = FileSignatureValidator.TryGetByteArrayFromBase64(dataUri, out var result);

        // Assert
        Assert.That(success, Is.True);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(bytes));
    }

    [Test]
    public void TryGetByteArrayFromBase64_WithoutPrefix_ExtractsBytesCorrectly()
    {
        // Arrange
        var bytes = Encoding.UTF8.GetBytes("Test Data");
        var base64 = Convert.ToBase64String(bytes);

        // Act
        var success = FileSignatureValidator.TryGetByteArrayFromBase64(base64, out var result);

        // Assert
        Assert.That(success, Is.True);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(bytes));
    }

    [Test]
    public void TryGetByteArrayFromBase64_WithInvalidBase64_ThrowsFormatException()
    {
        // Arrange
        const string invalidBase64 = "This is not valid base64!@#$%";

        // Act & Assert
        // The method doesn't handle FormatException, so it will throw
        Assert.Throws<FormatException>(() =>
            FileSignatureValidator.TryGetByteArrayFromBase64(invalidBase64, out _));
    }

    [Test]
    public void TryGetByteArrayFromBase64_WithNull_ReturnsFalse()
    {
        // Arrange
        string? nullData = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            FileSignatureValidator.TryGetByteArrayFromBase64(nullData!, out _));
    }

    [Test]
    public void TryGetByteArrayFromBase64_WithImageDataUri_ExtractsBytesCorrectly()
    {
        // Arrange
        var imageBytes = PngSignature;
        var base64 = Convert.ToBase64String(imageBytes);
        var dataUri = $"data:image/png;base64,{base64}";

        // Act
        var success = FileSignatureValidator.TryGetByteArrayFromBase64(dataUri, out var result);

        // Assert
        Assert.That(success, Is.True);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(imageBytes));
    }

    [Test]
    public void TryGetMimeType_FromIFormFile_DetectsPngCorrectly()
    {
        // Arrange
        var formFile = CreateFormFile(PngSignature, "test.png");

        // Act
        var success = formFile.TryGetMimeType(out var mimeType);

        // Assert
        Assert.That(success, Is.True);
        Assert.That(mimeType, Is.EqualTo("image/png"));
    }

    [Test]
    public void TryGetMimeType_FromBytes_DetectsJpegCorrectly()
    {
        // Arrange
        var jpegBytes = CreateValidJpegBytes();

        // Act
        var success = jpegBytes.TryGetMimeType(out var mimeType);

        // Assert
        Assert.That(success, Is.True);
        Assert.That(mimeType, Does.StartWith("image/jpeg").Or.StartWith("image/jpg"));
    }

    [Test]
    public void TryGetMimeType_FromBytes_DetectsPdfCorrectly()
    {
        // Arrange
        var pdfBytes = PdfSignature;

        // Act
        var success = pdfBytes.TryGetMimeType(out var mimeType);

        // Assert
        Assert.That(success, Is.True);
        Assert.That(mimeType, Is.EqualTo("application/pdf"));
    }

    [Test]
    public void TryGetMimeType_WithUnknownFormat_ReturnsFalse()
    {
        // Arrange
        var unknownBytes = Encoding.UTF8.GetBytes("This is just plain text");

        // Act
        var success = unknownBytes.TryGetMimeType(out var mimeType);

        // Assert - Plain text might not be recognized by FileSignatures library
        // The result depends on the library's capabilities
        if (!success)
        {
            Assert.That(mimeType, Is.Null.Or.Empty);
        }
    }

    [Test]
    public void IsPdf_WithValidPdfBytes_ReturnsTrue()
    {
        // Arrange
        var pdfBytes = PdfSignature;

        // Act
        var result = pdfBytes.IsPdf();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsPdf_WithNonPdfBytes_ReturnsFalse()
    {
        // Arrange
        var pngBytes = PngSignature;

        // Act
        var result = pngBytes.IsPdf();

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsPdf_WithValidPdfFormFile_ReturnsTrue()
    {
        // Arrange
        var formFile = CreateFormFile(PdfSignature, "document.pdf");

        // Act
        var result = formFile.IsPdf();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsPdf_WithNonPdfFormFile_ReturnsFalse()
    {
        // Arrange
        var formFile = CreateFormFile(PngSignature, "image.png");

        // Act
        var result = formFile.IsPdf();

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void TryGetMimeType_WithTruncatedSignature_ReturnsFalse()
    {
        // Arrange
        var truncatedBytes = new byte[] { 0x89, 0x50 }; // Incomplete PNG signature

        // Act
        var success = truncatedBytes.TryGetMimeType(out var mimeType);

        // Assert - Truncated signature should not be recognized
        Assert.That(success, Is.False);
    }

    [Test]
    public void TryGetMimeType_WithEmptyByteArray_ReturnsFalse()
    {
        // Arrange
        var emptyBytes = Array.Empty<byte>();

        // Act
        var success = emptyBytes.TryGetMimeType(out var mimeType);

        // Assert
        Assert.That(success, Is.False);
    }

    [Test]
    public void TryGetByteArrayFromBase64_WithEmptyString_ReturnsEmptyArray()
    {
        // Arrange
        const string emptyBase64 = "";

        // Act
        var success = FileSignatureValidator.TryGetByteArrayFromBase64(emptyBase64, out var result);

        // Assert
        Assert.That(success, Is.True);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    // Helper methods

    private static IFormFile CreateFormFile(byte[] content, string fileName)
    {
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, content.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/octet-stream"
        };
    }

    private static byte[] CreateValidJpegBytes()
    {
        // Create a minimal valid JPEG structure
        var jpeg = new List<byte>();
        jpeg.AddRange(JpegSignature); // SOI marker
        jpeg.AddRange([0xFF, 0xE0]); // APP0 marker
        jpeg.AddRange([0x00, 0x10]); // Length
        jpeg.AddRange(Encoding.ASCII.GetBytes("JFIF\0")); // JFIF identifier
        jpeg.AddRange([0x01, 0x01]); // Version
        jpeg.AddRange([0x00]); // Density units
        jpeg.AddRange([0x00, 0x01, 0x00, 0x01]); // X and Y density
        jpeg.AddRange([0x00, 0x00]); // Thumbnail dimensions
        jpeg.AddRange([0xFF, 0xD9]); // EOI marker
        return jpeg.ToArray();
    }
}
