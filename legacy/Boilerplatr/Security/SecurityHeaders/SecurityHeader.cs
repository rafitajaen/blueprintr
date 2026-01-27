using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Boilerplatr.Security.SecurityHeaders;
public static class SecurityHeader
{
    public static bool TryGetInlineScriptHash(string? script, [NotNullWhen(true)] out string sha)
    {
        sha = string.Empty;

        if (!string.IsNullOrWhiteSpace(script))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(script);
            byte[] hash = SHA256.HashData(bytes);
            string base64Hash = Convert.ToBase64String(hash);
            sha = $"sha256-{base64Hash}";
            return true;
        }

        return false;

    }
}
