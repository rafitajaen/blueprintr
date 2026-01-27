namespace Blueprintr.Endpoints.Tests;

[TestFixture]
public class EndpointExtensionsTests
{
    [Test]
    public void GetEndpointName_WithLeadingSlash_RemovesSlash()
    {
        // Arrange
        var endpointPath = "/api/users";

        // Act
        var result = endpointPath.GetEndpointName();

        // Assert
        Assert.That(result, Is.EqualTo("api-users"));
    }

    [Test]
    public void GetEndpointName_WithMultipleSlashes_ReplacesWithHyphens()
    {
        // Arrange
        var endpointPath = "/api/products/categories";

        // Act
        var result = endpointPath.GetEndpointName();

        // Assert
        Assert.That(result, Is.EqualTo("api-products-categories"));
    }

    [Test]
    public void GetEndpointName_WithNoLeadingSlash_WorksCorrectly()
    {
        // Arrange
        var endpointPath = "health";

        // Act
        var result = endpointPath.GetEndpointName();

        // Assert
        Assert.That(result, Is.EqualTo("health"));
    }

    [Test]
    public void GetEndpointName_WithEmptyString_ReturnsEmptyString()
    {
        // Arrange
        var endpointPath = "";

        // Act
        var result = endpointPath.GetEndpointName();

        // Assert
        Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void GetEndpointName_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        string? endpointPath = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => endpointPath!.GetEndpointName());
    }

    [Test]
    public void GetEndpointName_WithOnlySlash_ReturnsEmptyString()
    {
        // Arrange
        var endpointPath = "/";

        // Act
        var result = endpointPath.GetEndpointName();

        // Assert
        Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void GetEndpointName_WithTrailingSlash_KeepsInternalStructure()
    {
        // Arrange
        var endpointPath = "/api/users/";

        // Act
        var result = endpointPath.GetEndpointName();

        // Assert
        Assert.That(result, Is.EqualTo("api-users-"));
    }

    [TestCase("/api/users", "api-users")]
    [TestCase("/api/products/categories", "api-products-categories")]
    [TestCase("/health", "health")]
    [TestCase("status", "status")]
    [TestCase("/v1/api/customers/orders", "v1-api-customers-orders")]
    public void GetEndpointName_VariousPaths_ReturnsExpectedNames(string path, string expected)
    {
        // Act
        var result = path.GetEndpointName();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}
