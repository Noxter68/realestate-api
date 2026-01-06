using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Api.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
  public string Hash(string password)
  {
    // Generate a random salt
    byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

    // Hash with PBKDF2
    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
      password: password,
      salt: salt,
      prf: KeyDerivationPrf.HMACSHA256,
      iterationCount: 1000000,
      numBytesRequested: 256 / 8
    ));

    return $"{Convert.ToBase64String(salt)}.{hashed}";
  }

  public bool Verify(string password, string hash)
  {
    try
    {
      var parts = hash.Split('.');
      if (parts.Length != 2) return false;

      var salt = Convert.FromBase64String(parts[0]);
      var storedHash = parts[1];

      string computedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
               password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8));

      return computedHash == storedHash;
    }
    catch
    {
      return false;
    }
  }
}