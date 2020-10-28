using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace CRM.Server.Data
{
	public class PasswordHasher : IDisposable
	{
		private readonly RandomNumberGenerator rng;
		private readonly RijndaelManaged symKey;
		private const int keySize = 128 / 8;
		private readonly int iterCount;
		private bool disposedValue;

		public PasswordHasher()
		{
			var options = new PasswordHasherOptions();
			rng = RandomNumberGenerator.Create();
			iterCount = options.IterationCount;
			symKey = new RijndaelManaged
			{
				Mode = CipherMode.CBC,
				Padding = PaddingMode.PKCS7
			};
		}

		public string Encrypt(string password, object obj)
		{
			// Convert the object to encrypt the password with to string
			string plainText = JsonSerializer.Serialize(obj);

			// Creates the salt for the encryption key
			byte[]? salt = new byte[keySize];
			rng.GetBytes(salt);

			// Creates the iv for the encryptor
			byte[]? ivBytes = new byte[keySize];
			rng.GetBytes(ivBytes);

			// Gets the byte array from the password string
			byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
			byte[] key = KeyDerivation.Pbkdf2(plainText, salt, KeyDerivationPrf.HMACSHA256, iterCount, keySize);

			// Encrypts the password
			using ICryptoTransform encryptor = symKey.CreateEncryptor(key, ivBytes);
			using var memoryStream = new MemoryStream();
			using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
			cryptoStream.Write(passwordBytes, 0, passwordBytes.Length);
			cryptoStream.FlushFinalBlock();

			// Create the final bytes as a concatenation of the salt bytes,
			// the iv bytes and the cipher bytes, then returns it as a string
			byte[] cipherBytes = salt;
			cipherBytes = cipherBytes.Concat(ivBytes).ToArray();
			cipherBytes = cipherBytes.Concat(memoryStream.ToArray()).ToArray();
			return Convert.ToBase64String(cipherBytes);
		}

		public string Decrypt(string cipherText, object obj)
		{
			// Convert the object to decrypt the password with to string
			string plainText = JsonSerializer.Serialize(obj);

			// Get the complete stream of bytes that represent:
			// [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
			byte[] cipherBytes = Convert.FromBase64String(cipherText);

			// Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
			byte[] salt = cipherBytes.Take(keySize).ToArray();

			// Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
			byte[] ivBytes = cipherBytes.Skip(keySize).Take(keySize).ToArray();

			// Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
			byte[] cipherTextBytes = cipherBytes.Skip(keySize * 2).Take(cipherBytes.Length - (keySize * 2)).ToArray();
			byte[] key = KeyDerivation.Pbkdf2(plainText, salt, KeyDerivationPrf.HMACSHA256, iterCount, keySize);

			// Decrypts the password and returns it as a string
			using ICryptoTransform decryptor = symKey.CreateDecryptor(key, ivBytes);
			using var memoryStream = new MemoryStream(cipherTextBytes);
			using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
			byte[]? passwordBytes = new byte[cipherTextBytes.Length];
			int decryptedByteCount = cryptoStream.Read(passwordBytes, 0, passwordBytes.Length);
			return Encoding.UTF8.GetString(passwordBytes, 0, decryptedByteCount);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					rng.Dispose();
					symKey.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
