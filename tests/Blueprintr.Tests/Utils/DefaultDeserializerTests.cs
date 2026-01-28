using System.Text.Json;
using Blueprintr.Utils;
using NodaTime;
using NUnit.Framework;

namespace Blueprintr.Tests.Utils;

/// <summary>
/// Tests for <see cref="DefaultDeserializer"/> JSON deserialization options.
/// </summary>
[TestFixture]
public class DefaultDeserializerTests
{
    /// <summary>
    /// Test data class for deserialization tests.
    /// </summary>
    private class TestPerson
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
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

    [Test]
    public void Options_AllowsTrailingCommas()
    {
        // Arrange
        var jsonWithTrailingComma = """
        {
            "firstName": "John",
            "lastName": "Doe",
            "age": 30,
        }
        """;

        // Act
        var person = JsonSerializer.Deserialize<TestPerson>(jsonWithTrailingComma, DefaultDeserializer.Options);

        // Assert
        Assert.That(person, Is.Not.Null);
        Assert.That(person!.FirstName, Is.EqualTo("John"));
        Assert.That(person.LastName, Is.EqualTo("Doe"));
        Assert.That(person.Age, Is.EqualTo(30));
    }

    [Test]
    public void Options_IsCaseInsensitive()
    {
        // Arrange - Test various case combinations
        var jsonPascalCase = """{"FirstName": "John", "LastName": "Doe", "Age": 25}""";
        var jsonUpperCase = """{"FIRSTNAME": "Jane", "LASTNAME": "Smith", "AGE": 35}""";
        var jsonMixedCase = """{"firstName": "Bob", "LASTNAME": "Jones", "aGe": 40}""";

        // Act
        var personPascal = JsonSerializer.Deserialize<TestPerson>(jsonPascalCase, DefaultDeserializer.Options);
        var personUpper = JsonSerializer.Deserialize<TestPerson>(jsonUpperCase, DefaultDeserializer.Options);
        var personMixed = JsonSerializer.Deserialize<TestPerson>(jsonMixedCase, DefaultDeserializer.Options);

        // Assert
        Assert.That(personPascal, Is.Not.Null);
        Assert.That(personPascal!.FirstName, Is.EqualTo("John"));
        Assert.That(personPascal.Age, Is.EqualTo(25));

        Assert.That(personUpper, Is.Not.Null);
        Assert.That(personUpper!.FirstName, Is.EqualTo("Jane"));
        Assert.That(personUpper.Age, Is.EqualTo(35));

        Assert.That(personMixed, Is.Not.Null);
        Assert.That(personMixed!.FirstName, Is.EqualTo("Bob"));
        Assert.That(personMixed.LastName, Is.EqualTo("Jones"));
        Assert.That(personMixed.Age, Is.EqualTo(40));
    }

    [Test]
    public void Options_AllowsComments()
    {
        // Arrange
        var jsonWithComments = """
        {
            // This is a single-line comment
            "firstName": "John",
            /* This is a
               multi-line comment */
            "lastName": "Doe",
            "age": 30
        }
        """;

        // Act
        var person = JsonSerializer.Deserialize<TestPerson>(jsonWithComments, DefaultDeserializer.Options);

        // Assert
        Assert.That(person, Is.Not.Null);
        Assert.That(person!.FirstName, Is.EqualTo("John"));
        Assert.That(person.LastName, Is.EqualTo("Doe"));
        Assert.That(person.Age, Is.EqualTo(30));
    }

    [Test]
    public void Options_DeserializesNodaTimeTypes()
    {
        // Arrange
        var json = """
        {
            "name": "Annual Conference",
            "eventDate": "2024-06-15",
            "createdAt": "2024-01-01T12:00:00Z",
            "startTime": "2024-06-15T09:30:00"
        }
        """;

        // Act
        var testEvent = JsonSerializer.Deserialize<TestEvent>(json, DefaultDeserializer.Options);

        // Assert
        Assert.That(testEvent, Is.Not.Null);
        Assert.That(testEvent!.Name, Is.EqualTo("Annual Conference"));
        Assert.That(testEvent.EventDate, Is.EqualTo(new LocalDate(2024, 6, 15)));
        Assert.That(testEvent.CreatedAt, Is.EqualTo(Instant.FromUtc(2024, 1, 1, 12, 0, 0)));
        Assert.That(testEvent.StartTime, Is.EqualTo(new LocalDateTime(2024, 6, 15, 9, 30, 0)));
    }

    [Test]
    public void Options_HandlesCamelCaseProperties()
    {
        // Arrange - camelCase JSON should map correctly
        var json = """
        {
            "firstName": "Alice",
            "lastName": "Wonder",
            "age": 28
        }
        """;

        // Act
        var person = JsonSerializer.Deserialize<TestPerson>(json, DefaultDeserializer.Options);

        // Assert
        Assert.That(person, Is.Not.Null);
        Assert.That(person!.FirstName, Is.EqualTo("Alice"));
        Assert.That(person.LastName, Is.EqualTo("Wonder"));
        Assert.That(person.Age, Is.EqualTo(28));
    }
}
