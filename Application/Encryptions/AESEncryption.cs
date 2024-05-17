using System;
using System.Text;
using System.Security.Cryptography;


namespace Application.Encryptions
{
    public class AESEncryption : Encrypt
    {
        private readonly int _blocksize = 128;
        private readonly byte[] _iv = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private readonly string _encryptionKey;

        public AESEncryption(string encryptionKey)
        {
            _encryptionKey = encryptionKey;
        }

        private string EncryptText(string plainText, string Password)
        {
            using (SymmetricAlgorithm crypt = Aes.Create())
            {
                using (HashAlgorithm hash = MD5.Create())
                {
                    crypt.BlockSize = _blocksize;
                    crypt.Key = hash.ComputeHash(Encoding.UTF8.GetBytes(Password));
                    crypt.IV = _iv;
                }

                byte[] bytes = Encoding.UTF8.GetBytes(plainText); //change encoding to UTF8
                using(var memoryStream = new System.IO.MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                    }
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        private string DecryptText(string encryptedText, string password)
        {
            byte[] bytes = Convert.FromBase64String(encryptedText);
            using (SymmetricAlgorithm crypt = Aes.Create())
            {
                using (HashAlgorithm hash = MD5.Create())
                {
                    crypt.Key = hash.ComputeHash(Encoding.UTF8.GetBytes(password)); // Change encoding to UTF8
                    crypt.IV = _iv;

                    using (var memoryStream = new System.IO.MemoryStream(bytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            byte[] decryptedBytes = new byte[bytes.Length];
                            int decryptedLength = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                            // Trim null characters from the decrypted string
                            string decryptedString = Encoding.UTF8.GetString(decryptedBytes, 0, decryptedLength);
                            return decryptedString;
                        }
                    }
                }
            }
        }

        public override string EncryptPassword(string password)
        {
            return EncryptText(password, _encryptionKey);
        }

        public override string DecryptPassword(string encryptedPassword)
        {
            return DecryptText(encryptedPassword, _encryptionKey);
        }

    }
}
