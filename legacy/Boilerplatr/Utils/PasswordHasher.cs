using System.Security.Cryptography;

namespace Boilerplatr.Utils;

public sealed class PasswordHasher
{
    private const int SaltSize = 16;
    private const int OutputLength = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    /* HexString Methods */
    public static string GenerateSaltHexString(int saltSize = SaltSize) => Convert.ToHexString(RandomNumberGenerator.GetBytes(saltSize));
    public static string HashHexString(string password, byte[] salt, int iterations = Iterations, int outputLength = OutputLength, HashAlgorithmName? hashAlgorithm = null)
    {
        return Convert.ToHexString
        (
            inArray: Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm ?? HashAlgorithm, outputLength)
        );
    }
    public static bool Verify(string? password, string? passwordHash, string? passwordSalt, int iterations = Iterations, int hashSize = OutputLength, HashAlgorithmName? algorithm = null)
    {
        if (password is null || passwordHash is null || passwordSalt is null)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals
        (
            left: Convert.FromHexString(passwordHash),
            right: Rfc2898DeriveBytes.Pbkdf2
            (
                password: password,
                salt: Convert.FromHexString(passwordSalt),
                iterations: iterations,
                hashAlgorithm: algorithm ?? HashAlgorithm,
                outputLength: hashSize
            )
        );
    }

    /* Byte Array Methods */
    public static byte[] GenerateSalt(int saltSize = SaltSize) => RandomNumberGenerator.GetBytes(saltSize);
    public static byte[] Hash(string password, byte[] salt, int iterations = Iterations, int outputLength = OutputLength, HashAlgorithmName? hashAlgorithm = null)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm ?? HashAlgorithm, outputLength);
    }
    public static bool Verify(string? password, byte[]? passwordHash, byte[]? passwordSalt, int iterations = Iterations, int outputLength = OutputLength, HashAlgorithmName? hashAlgorithm = null)
    {
        if (password is null || passwordHash is null || passwordSalt is null)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals
        (
            left: passwordHash,
            right: Rfc2898DeriveBytes.Pbkdf2(password, passwordSalt, iterations, hashAlgorithm ?? HashAlgorithm, outputLength)
        );
    }
}