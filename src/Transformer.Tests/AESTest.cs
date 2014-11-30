using System.IO;
using NUnit.Framework;
using Transformer.Core.Cryptography;

namespace Transformer.Tests
{
    [TestFixture]
    public class AESTest
    {
        const string Password = "test";
        const string TextToEncrypt = "blub";

        [Test]
        public void Encrypt_From_Password()
        {
            var encryptedFromPassword = AES.Encrypt(TextToEncrypt, Password);
            var decripted = AES.Decrypt(encryptedFromPassword, Password);

            Assert.AreEqual(TextToEncrypt, decripted);
        }

        [Test]
        public void Encrypt_From_PasswordFile()
        {
            var tempFileName = Path.GetTempFileName();

            File.WriteAllText(tempFileName, Password);
            var passwordFromFile = File.ReadAllText(tempFileName);

            var encryptedFromFile = AES.Encrypt(TextToEncrypt, passwordFromFile);
            var decrypted2 = AES.Decrypt(encryptedFromFile, passwordFromFile);
            var decrypted3 = AES.Decrypt(encryptedFromFile, passwordFromFile);
            var decrypted4 = AES.Decrypt(encryptedFromFile, Password);

            Assert.AreEqual(TextToEncrypt, decrypted2);
            Assert.AreEqual(TextToEncrypt, decrypted3);
            Assert.AreEqual(TextToEncrypt, decrypted4);

            Assert.AreEqual(Password, passwordFromFile);

            File.Delete(tempFileName);
        }
    }
}