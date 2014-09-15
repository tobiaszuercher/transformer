using System.Security.Cryptography;
using Moq;
using NUnit.Framework;
using PowerDeploy.Transformer;

namespace Transformer.Tests
{
    [TestFixture]
    public class EnvironmentEncrypterTest
    {
        [Test]
        public void Encrypt_Environment()
        {
            var env = new Environment();
            env.Variables.Add(new Variable() { DoEncrypt = true, Name = "encrypted-var", Value = "secret" });
            env.Variables.Add(new Variable() { DoEncrypt = false, Name = "unsecure", Value = "unsecure" });

            var environmentProviderMock = new Mock<IEnvironmentProvider>();
            environmentProviderMock.Setup(prov => prov.GetEnvironment(It.IsAny<string>())).Returns(env);

            var encrypter = new EnvironmentEncrypter(environmentProviderMock.Object, "password");
            encrypter.EncryptAllEnvironments();
        }

        [Test]
        public void Encrypt_Variable()
        {
            var env = new Environment();
            env.Variables.Add(new Variable() { DoEncrypt = true, Name = "encrypted-var", Value = "secret" });
            env.Variables.Add(new Variable() { DoEncrypt = true, Name = "another-secret-pw", Value = "secret" });
            env.Variables.Add(new Variable() { DoEncrypt = false, Name = "unsecure", Value = "unsecure" });

            env.EncryptVariables("gugus");

            Assert.AreNotEqual("secret", env["encrypted-var"].Value);
            Assert.AreNotEqual("secret", env["another-secret-pw"].Value);
        }

        [Test]
        public void Decrypt_Variable()
        {
            var env = new Environment();
            env.Variables.Add(new Variable() { DoEncrypt = true, Name = "one", Value = "1" });
            env.Variables.Add(new Variable() { DoEncrypt = true, Name = "two", Value = "2" });
            env.EncryptVariables("super-duper-secret-pw!");

            Assert.AreNotEqual("1", env["one"].Value);
            Assert.AreNotEqual("2", env["two"].Value);

            env.DecryptVariables("super-duper-secret-pw!");

            Assert.AreEqual("1", env["one"].Value);
            Assert.AreEqual("2", env["two"].Value);
        }

        [Test]
        public void Decrypt_Variable_WrongPassowrd()
        {
            var env = new Environment();
            env.Variables.Add(new Variable() { DoEncrypt = true, Name = "firstname", Value = "Raymond" });
            env.Variables.Add(new Variable() { DoEncrypt = true, Name = "lastname", Value = "Redington" });
            env.EncryptVariables("super-duper-secret-pw!");

            Assert.Throws<CryptographicException>(() => env.DecryptVariables("wrong-pw!"));

            Assert.AreNotEqual("Raymond", env["firstname"].Value);
            Assert.AreNotEqual("Redington", env["lastname"].Value);
        }
    }
}