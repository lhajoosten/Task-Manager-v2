using System.Security.Cryptography;
using TaskManager.Application.Auth.Services;

namespace TaskManager.Persistence.Services;

public class PasswordService : IPasswordService
{
	private const int SaltSize = 128 / 8; // 128 bits
	private const int KeySize = 256 / 8; // 256 bits
	private const int Iterations = 10000;
	private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

	public string HashPassword(string password)
	{
		var salt = RandomNumberGenerator.GetBytes(SaltSize);
		var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, KeySize);

		return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
	}

	public bool VerifyPassword(string password, string hash)
	{
		try
		{
			var parts = hash.Split(':');
			if (parts.Length != 2)
				return false;

			var salt = Convert.FromBase64String(parts[0]);
			var storedHash = Convert.FromBase64String(parts[1]);

			var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, KeySize);

			return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
		}
		catch
		{
			return false;
		}
	}
}
