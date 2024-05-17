using System;
using System;
using System.Text;
using System.Security.Cryptography;

namespace Application.Encryptions
{
    public class RSAEncryption : Encrypt
    {
        private RSA _rsa;

        public RSAEncryption()
        {
            _rsa = RSA.Create(2048);
        }

        public override string EncryptPassword(string plaintext)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] encryptBytes = _rsa.Encrypt(plainBytes, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(encryptBytes);
        }
        public override string DecryptPassword(string encryptedText) 
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        public string getPublicKey() 
        {
            return _rsa.ToXmlString(false);
        }
    }
}
