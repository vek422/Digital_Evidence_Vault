using System.Configuration;
using System.Security.Cryptography;
using System.Text;

public static class SecurityHelper
{
    private static readonly string _secretKey;

    static SecurityHelper()
    {
        _secretKey = ConfigurationManager.AppSettings["SecretKey"] ?? "DEFAULT_SECRET_KEY";
    }

    /// <summary>
    /// Computes SHA-256 hash of a file using streaming (low memory usage)
    /// </summary>
    public static string ComputeFileHash(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1024 * 1024);

        byte[] hashBytes = sha256.ComputeHash(stream);
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// Generates HMAC-SHA256 integrity signature binding content hash and timestamp
    /// </summary>
    public static string GenerateIntegritySignature(string fileHash, long ticks)
    {
        string data = $"{fileHash}:{ticks}";
        return ComputeHmac(data, _secretKey);
    }

    /// <summary>
    /// Verifies HMAC-SHA256 integrity signature
    /// </summary>
    public static bool VerifyIntegritySignature(string fileHash, long ticks, string expectedSignature)
    {
        string computedSignature = GenerateIntegritySignature(fileHash, ticks);
        return string.Equals(computedSignature, expectedSignature, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Computes HMAC-SHA256 of data using the provided key
    /// </summary>
    private static string ComputeHmac(string data, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        byte[] hashBytes = hmac.ComputeHash(dataBytes);
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// Computes SHA-256 hash of arbitrary data
    /// </summary>
    public static string ComputeHash(string data)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        byte[] hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }
}
