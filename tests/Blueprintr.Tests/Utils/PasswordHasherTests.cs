using Blueprintr.Utils;
using NUnit.Framework;
using System.Security.Cryptography;

namespace Blueprintr.Tests.Utils;

[TestFixture]
public class PasswordHasherTests
{
    [Test]
    public void GenerateSaltHexString_ShouldReturnNonEmptyString()
    {
        var salt = PasswordHasher.GenerateSaltHexString();
        
        Assert.That(salt, Is.Not.Null);
        Assert.That(salt, Is.Not.Empty);
        Assert.That(salt.Length, Is.EqualTo(32)); // 16 bytes = 32 hex characters
    }

    [Test]
    public void GenerateSalt_ShouldReturnByteArray()
    {
        var salt = PasswordHasher.GenerateSalt();
        
        Assert.That(salt, Is.Not.Null);
        Assert.That(salt.Length, Is.EqualTo(16));
    }

    [Test]
    public void HashHexString_ShouldReturnConsistentHash()
    {
        var password = "TestPassword123!";
        var salt = PasswordHasher.GenerateSalt();
        
        var hash1 = PasswordHasher.HashHexString(password, salt);
        var hash2 = PasswordHasher.HashHexString(password, salt);
        
        Assert.That(hash1, Is.EqualTo(hash2));
        Assert.That(hash1.Length, Is.EqualTo(64)); // 32 bytes = 64 hex characters
    }

    [Test]
    public void Hash_ShouldReturnConsistentHash()
    {
        var password = "TestPassword123!";
        var salt = PasswordHasher.GenerateSalt();
        
        var hash1 = PasswordHasher.Hash(password, salt);
        var hash2 = PasswordHasher.Hash(password, salt);
        
        Assert.That(hash1, Is.EqualTo(hash2));
        Assert.That(hash1.Length, Is.EqualTo(32));
    }

    [Test]
    public void Verify_HexString_ValidPassword_ShouldReturnTrue()
    {
        var password = "TestPassword123!";
        var salt = PasswordHasher.GenerateSaltHexString();
        var hash = PasswordHasher.HashHexString(password, Convert.FromHexString(salt));
        
        var result = PasswordHasher.Verify(password, hash, salt);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void Verify_HexString_InvalidPassword_ShouldReturnFalse()
    {
        var password = "TestPassword123!";
        var wrongPassword = "WrongPassword456!";
        var salt = PasswordHasher.GenerateSaltHexString();
        var hash = PasswordHasher.HashHexString(password, Convert.FromHexString(salt));
        
        var result = PasswordHasher.Verify(wrongPassword, hash, salt);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void Verify_ByteArray_ValidPassword_ShouldReturnTrue()
    {
        var password = "TestPassword123!";
        var salt = PasswordHasher.GenerateSalt();
        var hash = PasswordHasher.Hash(password, salt);
        
        var result = PasswordHasher.Verify(password, hash, salt);
        
        Assert.That(result, Is.True);
    }

    [Test]
    public void Verify_ByteArray_InvalidPassword_ShouldReturnFalse()
    {
        var password = "TestPassword123!";
        var wrongPassword = "WrongPassword456!";
        var salt = PasswordHasher.GenerateSalt();
        var hash = PasswordHasher.Hash(password, salt);
        
        var result = PasswordHasher.Verify(wrongPassword, hash, salt);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void Verify_NullPassword_ShouldReturnFalse()
    {
        var salt = PasswordHasher.GenerateSaltHexString();
        var hash = PasswordHasher.HashHexString("test", Convert.FromHexString(salt));
        
        var result = PasswordHasher.Verify(null, hash, salt);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void Verify_NullHash_ShouldReturnFalse()
    {
        var salt = PasswordHasher.GenerateSaltHexString();
        
        var result = PasswordHasher.Verify("test", null, salt);
        
        Assert.That(result, Is.False);
    }

    [Test]
    public void Verify_NullSalt_ShouldReturnFalse()
    {
        var password = "test";
        var salt = PasswordHasher.GenerateSaltHexString();
        var hash = PasswordHasher.HashHexString(password, Convert.FromHexString(salt));
        
        var result = PasswordHasher.Verify(password, hash, null);
        
        Assert.That(result, Is.False);
    }
}
