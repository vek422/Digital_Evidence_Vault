using System.Security.Cryptography;
using System.Text;

public static class PasswordHelper
{

    public static string HashPassword(string password, string salt)
    {
        string rawData = password + salt;
        using (var sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(rawData);
            byte[] hashBytes = sha256.ComputeHash(bytes);

            return Convert.ToHexString(hashBytes);
        }
    }
    public static string GenerateSalt(int size = 16)
    {
        byte[] saltBytes = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToHexString(saltBytes);
    }

}