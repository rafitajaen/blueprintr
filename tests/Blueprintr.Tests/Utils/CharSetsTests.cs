using Blueprintr.Utils;
using NUnit.Framework;

namespace Blueprintr.Tests.Utils;

[TestFixture]
public class CharSetsTests
{
    [Test]
    public void Binary_ShouldContainOnlyZeroAndOne()
    {
        Assert.That(CharSets.Binary, Is.EqualTo("01"));
        Assert.That(CharSets.Binary.Length, Is.EqualTo(2));
    }

    [Test]
    public void Hexadecimal_ShouldContainCorrectCharacters()
    {
        Assert.That(CharSets.Hexadecimal, Is.EqualTo("0123456789abcdef"));
        Assert.That(CharSets.Hexadecimal.Length, Is.EqualTo(16));
    }

    [Test]
    public void Numeric_ShouldContainDigits()
    {
        Assert.That(CharSets.Numeric, Is.EqualTo("0123456789"));
        Assert.That(CharSets.Numeric.Length, Is.EqualTo(10));
    }

    [Test]
    public void Octal_ShouldContainDigitsZeroToSeven()
    {
        Assert.That(CharSets.Octal, Is.EqualTo("01234567"));
        Assert.That(CharSets.Octal.Length, Is.EqualTo(8));
    }

    [Test]
    public void Alphabetic_ShouldContainAllLetters()
    {
        Assert.That(CharSets.Alphabetic.Length, Is.EqualTo(52));
        Assert.That(CharSets.Alphabetic, Does.Contain("a"));
        Assert.That(CharSets.Alphabetic, Does.Contain("z"));
        Assert.That(CharSets.Alphabetic, Does.Contain("A"));
        Assert.That(CharSets.Alphabetic, Does.Contain("Z"));
    }

    [Test]
    public void LowerAlphabetic_ShouldContainOnlyLowercase()
    {
        Assert.That(CharSets.LowerAlphabetic.Length, Is.EqualTo(26));
        Assert.That(CharSets.LowerAlphabetic, Does.Contain("a"));
        Assert.That(CharSets.LowerAlphabetic, Does.Contain("z"));
        Assert.That(CharSets.LowerAlphabetic, Does.Not.Contain("A"));
    }

    [Test]
    public void UpperAlphabetic_ShouldContainOnlyUppercase()
    {
        Assert.That(CharSets.UpperAlphabetic.Length, Is.EqualTo(26));
        Assert.That(CharSets.UpperAlphabetic, Does.Contain("A"));
        Assert.That(CharSets.UpperAlphabetic, Does.Contain("Z"));
        Assert.That(CharSets.UpperAlphabetic, Does.Not.Contain("a"));
    }

    [Test]
    public void Alphanumeric_ShouldContainDigitsAndLetters()
    {
        Assert.That(CharSets.Alphanumeric.Length, Is.EqualTo(62));
        Assert.That(CharSets.Alphanumeric, Does.Contain("0"));
        Assert.That(CharSets.Alphanumeric, Does.Contain("9"));
        Assert.That(CharSets.Alphanumeric, Does.Contain("a"));
        Assert.That(CharSets.Alphanumeric, Does.Contain("Z"));
    }

    [Test]
    public void LowerAlphanumeric_ShouldContainDigitsAndLowercase()
    {
        Assert.That(CharSets.LowerAlphanumeric.Length, Is.EqualTo(36));
        Assert.That(CharSets.LowerAlphanumeric, Does.Contain("0"));
        Assert.That(CharSets.LowerAlphanumeric, Does.Contain("9"));
        Assert.That(CharSets.LowerAlphanumeric, Does.Contain("a"));
        Assert.That(CharSets.LowerAlphanumeric, Does.Not.Contain("A"));
    }

    [Test]
    public void UpperAlphanumeric_ShouldContainDigitsAndUppercase()
    {
        Assert.That(CharSets.UpperAlphanumeric.Length, Is.EqualTo(36));
        Assert.That(CharSets.UpperAlphanumeric, Does.Contain("0"));
        Assert.That(CharSets.UpperAlphanumeric, Does.Contain("9"));
        Assert.That(CharSets.UpperAlphanumeric, Does.Contain("A"));
        Assert.That(CharSets.UpperAlphanumeric, Does.Not.Contain("a"));
    }
}
