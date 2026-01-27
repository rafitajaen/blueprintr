using System.Security.Cryptography;

namespace Blueprintr.Utils;

/// <summary>
/// Provides secure password hashing and verification using PBKDF2 (Password-Based Key Derivation Function 2).
/// </summary>
/// <remarks>
/// This class uses industry-standard cryptographic practices including:
/// - PBKDF2 algorithm with SHA512
/// - 100,000 iterations (recommended by OWASP as of 2023)
/// - 16-byte salt
/// - 32-byte output length
/// - Constant-time comparison to prevent timing attacks
/// Added in version 1.0.0.
/// </remarks>
public sealed class PasswordHasher
{
    private const int SaltSize = 16;
    private const int OutputLength = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    /* HexString Methods */
    
    /// <summary>
    /// Generates a cryptographically secure random salt as a hexadecimal string.
    /// </summary>
    /// <param name="saltSize">The size of the salt in bytes. Defaults to 16 bytes.</param>
    /// <returns>A hexadecimal string representation of the generated salt.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static string GenerateSaltHexString(int saltSize = SaltSize) => Convert.ToHexString(RandomNumberGenerator.GetBytes(saltSize));
    
    /// <summary>
    /// Hashes a password using PBKDF2 and returns the result as a hexadecimal string.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="salt">The salt to use for hashing.</param>
    /// <param name="iterations">The number of iterations for PBKDF2. Defaults to 100,000.</param>
    /// <param name="outputLength">The desired length of the hash output in bytes. Defaults to 32 bytes.</param>
    /// <param name="hashAlgorithm">The hash algorithm to use. Defaults to SHA512.</param>
    /// <returns>A hexadecimal string representation of the password hash.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static string HashHexString(string password, byte[] salt, int iterations = Iterations, int outputLength = OutputLength, HashAlgorithmName? hashAlgorithm = null)
    {
        return Convert.ToHexString
        (
            inArray: Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm ?? HashAlgorithm, outputLength)
        );
    }
    
    /// <summary>
    /// Verifies a password against a stored hash and salt using constant-time comparison.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="passwordHash">The stored password hash as a hexadecimal string.</param>
    /// <param name="passwordSalt">The stored salt as a hexadecimal string.</param>
    /// <param name="iterations">The number of iterations used when creating the hash. Defaults to 100,000.</param>
    /// <param name="hashSize">The size of the hash in bytes. Defaults to 32 bytes.</param>
    /// <param name="algorithm">The hash algorithm used. Defaults to SHA512.</param>
    /// <returns>True if the password matches the hash; otherwise, false.</returns>
    /// <remarks>
    /// Uses constant-time comparison to prevent timing attacks.
    /// Returns false if any parameter is null.
    /// Added in version 1.0.0.
    /// </remarks>
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
    
    /// <summary>
    /// Generates a cryptographically secure random salt as a byte array.
    /// </summary>
    /// <param name="saltSize">The size of the salt in bytes. Defaults to 16 bytes.</param>
    /// <returns>A byte array containing the generated salt.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static byte[] GenerateSalt(int saltSize = SaltSize) => RandomNumberGenerator.GetBytes(saltSize);
    
    /// <summary>
    /// Hashes a password using PBKDF2 and returns the result as a byte array.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="salt">The salt to use for hashing.</param>
    /// <param name="iterations">The number of iterations for PBKDF2. Defaults to 100,000.</param>
    /// <param name="outputLength">The desired length of the hash output in bytes. Defaults to 32 bytes.</param>
    /// <param name="hashAlgorithm">The hash algorithm to use. Defaults to SHA512.</param>
    /// <returns>A byte array containing the password hash.</returns>
    /// <remarks>Added in version 1.0.0.</remarks>
    public static byte[] Hash(string password, byte[] salt, int iterations = Iterations, int outputLength = OutputLength, HashAlgorithmName? hashAlgorithm = null)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm ?? HashAlgorithm, outputLength);
    }
    
    /// <summary>
    /// Verifies a password against a stored hash and salt using constant-time comparison.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="passwordHash">The stored password hash as a byte array.</param>
    /// <param name="passwordSalt">The stored salt as a byte array.</param>
    /// <param name="iterations">The number of iterations used when creating the hash. Defaults to 100,000.</param>
    /// <param name="outputLength">The size of the hash in bytes. Defaults to 32 bytes.</param>
    /// <param name="hashAlgorithm">The hash algorithm used. Defaults to SHA512.</param>
    /// <returns>True if the password matches the hash; otherwise, false.</returns>
    /// <remarks>
    /// Uses constant-time comparison to prevent timing attacks.
    /// Returns false if any parameter is null.
    /// Added in version 1.0.0.
    /// </remarks>
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
