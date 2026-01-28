using System.Text.Json;
using Blueprintr.Utils;
using NodaTime;
using NUnit.Framework;

namespace Blueprintr.Tests.Utils;

/// <summary>
/// Tests for <see cref="DefaultSerializer"/> JSON serialization options.
/// </summary>
[TestFixture]
public class DefaultSerializerTests
{
    /// <summary>
    /// Test data class for serialization tests.
    /// </summary>
    private class TestPerson
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public string? NullableField { get; set; }
    }

    /// <summary>
    /// Test data class with NodaTime types.
    /// </summary>
    private class TestEvent
    {
        public string? Name { get; set; }
        public LocalDate EventDate { get; set; }
        public Instant CreatedAt { get; set; }
        public LocalDateTime StartTime { get; set; }
    }

    /// <summary>
    /// Test data class with complex nested types.
    /// </summary>
    private class TestOrder
    {
        public string? OrderId { get; set; }
        public List<TestOrderItem>? Items { get; set; }
        public Dictionary<string, decimal>? Metadata { get; set; }
    }

    private class TestOrderItem
    {
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
    }

    [Test]
    public void Options_UsesCamelCaseNaming()
    {
        // Arrange
        var person = new TestPerson
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30
        };

        // Act
        var json = JsonSerializer.Serialize(person, DefaultSerializer.Options);

        // Assert
        Assert.That(json, Does.Contain("\"firstName\""));
        Assert.That(json, Does.Contain("\"lastName\""));
        Assert.That(json, Does.Contain("\"age\""));
        Assert.That(json, Does.Not.Contain("\"FirstName\""));
        Assert.That(json, Does.Not.Contain("\"LastName\""));
        Assert.That(json, Does.Not.Contain("\"Age\""));
    }

    [Test]
    public void Options_IgnoresNullValues()
    {
        // Arrange
        var person = new TestPerson
        {
            FirstName = "John",
            LastName = null,
            Age = null,
            NullableField = null
        };

        // Act
        var json = JsonSerializer.Serialize(person, DefaultSerializer.Options);

        // Assert
        Assert.That(json, Does.Contain("\"firstName\""));
        Assert.That(json, Does.Not.Contain("\"lastName\""));
        Assert.That(json, Does.Not.Contain("\"age\""));
        Assert.That(json, Does.Not.Contain("\"nullableField\""));
        Assert.That(json, Does.Not.Contain("null"));
    }

    [Test]
    public void Options_SerializesNodaTimeTypes()
    {
        // Arrange
        var testEvent = new TestEvent
        {
            Name = "Conference",
            EventDate = new LocalDate(2024, 6, 15),
            CreatedAt = Instant.FromUtc(2024, 1, 1, 12, 0, 0),
            StartTime = new LocalDateTime(2024, 6, 15, 9, 30, 0)
        };

        // Act
        var json = JsonSerializer.Serialize(testEvent, DefaultSerializer.Options);

        // Assert
        Assert.That(json, Does.Contain("\"eventDate\""));
        Assert.That(json, Does.Contain("\"createdAt\""));
        Assert.That(json, Does.Contain("\"startTime\""));
        Assert.That(json, Does.Contain("2024-06-15"));
        Assert.That(json, Does.Contain("2024-01-01T12:00:00Z"));

        // Verify the JSON can be parsed
        var document = JsonDocument.Parse(json);
        Assert.That(document.RootElement.GetProperty("eventDate").GetString(), Is.EqualTo("2024-06-15"));
    }

    [Test]
    public void Options_SerializesComplexTypes()
    {
        // Arrange
        var order = new TestOrder
        {
            OrderId = "ORD-12345",
            Items = new List<TestOrderItem>
            {
                new() { ProductName = "Widget A", Quantity = 3 },
                new() { ProductName = "Widget B", Quantity = 1 }
            },
            Metadata = new Dictionary<string, decimal>
            {
                { "TotalPrice", 99.99m },
                { "DiscountAmount", 10.00m }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(order, DefaultSerializer.Options);

        // Assert
        Assert.That(json, Does.Contain("\"orderId\""));
        Assert.That(json, Does.Contain("\"items\""));
        Assert.That(json, Does.Contain("\"productName\""));
        Assert.That(json, Does.Contain("\"quantity\""));

        // Verify dictionary keys are camelCase
        Assert.That(json, Does.Contain("\"totalPrice\""));
        Assert.That(json, Does.Contain("\"discountAmount\""));
        Assert.That(json, Does.Not.Contain("\"TotalPrice\""));
        Assert.That(json, Does.Not.Contain("\"DiscountAmount\""));

        // Verify it's valid JSON by parsing
        var document = JsonDocument.Parse(json);
        Assert.That(document.RootElement.GetProperty("items").GetArrayLength(), Is.EqualTo(2));
    }
}
