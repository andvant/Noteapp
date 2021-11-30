using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public string Encrypt(string text)
        {
            var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(_encryptionKey);

            byte[] encryptedBytes;
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(aes.IV);
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (var streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(text);
                    }
                }
                encryptedBytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string cipher)
        {
            var aes = Aes.Create();
            aes.Key = Convert.FromBase64String(_encryptionKey);

            byte[] encryptedBytes = Convert.FromBase64String(cipher);

            using (var memoryStream = new MemoryStream(encryptedBytes))
            {
                var buffer = new byte[aes.IV.Length];
                memoryStream.Read(buffer);
                aes.IV = buffer;
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }

        public static string GenerateEncryptionKey(string password, string salt)
        {
            // TODO: fix
            if (string.IsNullOrEmpty(salt)) return string.Empty;
            var pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt));
            var encryptionKey = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(encryptionKey);
        }
    }
}
