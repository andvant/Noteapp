using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Noteapp.Desktop.Security
{
    public class Protector
    {
        private string _encryptionKey;
        public Protector(string encryptionKey)
        {
            _encryptionKey = encryptionKey;
        }

        public async Task<string> Encrypt(string text)
        {
            var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(_encryptionKey);

            byte[] encryptedBytes;
            using (var memoryStream = new MemoryStream())
            {
                await memoryStream.WriteAsync(aes.IV);
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (var streamWriter = new StreamWriter(cryptoStream))
                    {
                        await streamWriter.WriteAsync(text);
                    }
                }
                encryptedBytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public async Task<string> Decrypt(string cipher)
        {
            var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(_encryptionKey);

            byte[] encryptedBytes = Convert.FromBase64String(cipher);

            using (var memoryStream = new MemoryStream(encryptedBytes))
            {
                var buffer = new byte[aes.IV.Length];
                await memoryStream.ReadAsync(buffer);
                aes.IV = buffer;
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        return await streamReader.ReadToEndAsync();
                    }
                }
            }
        }

        public static string GenerateEncryptionKey(string password, string salt)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt));
            var encryptionKey = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(encryptionKey);
        }
    }
}
