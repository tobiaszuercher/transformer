using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Transformer.Core;
using Transformer.Core.Model;
using Environment = Transformer.Core.Model.Environment;

namespace Transformer.Tests
{
    [TestFixture]
    public class CommandLineIntegrationTests
    {
        [Test]
        public void Transform_FileIsTransformed()
        {
            using (var dir = new TestFolder())
            {
                // arrange
                dir.AddFolder(SearchInParentFolderLocator.EnvironmentFolderName);
                dir.AddFile("unit.xml".RelativeTo(SearchInParentFolderLocator.EnvironmentFolderName), GetEnvironment().ToXml());
                dir.AddFile("app.template.config", "${firstname} ${lastname}");
                
                // act
                Transform("unit", dir.DirectoryInfo.FullName);

                // assert
                Assert.AreEqual("tobi zürcher", dir.ReadFile("app.config"));
            }
        }

        [Test]
        public void Transform_With_Not_Existing_Environment()
        {
            using (var dir = new TestFolder())
            {
                // arrange
                dir.AddFolder(SearchInParentFolderLocator.EnvironmentFolderName);
                dir.AddFile("unit.xml".RelativeTo(SearchInParentFolderLocator.EnvironmentFolderName), GetEnvironment().ToXml());
                dir.AddFile("app.template.config", "${firstname} ${lastname}");

                // act
                Transform("wrong", dir.DirectoryInfo.FullName);

                // assert
                //Assert.AreEqual("tobi zürcher", dir.ReadFile("app.config"));
            }
        }

        [Test]
        public void Transform_With_No_Path_Variable()
        {
            using (var dir = new TestFolder())
            {
                // arrange
                dir.AddFolder(SearchInParentFolderLocator.EnvironmentFolderName);

                // act
                Transform("unit", string.Empty);

                // assert
                Assert.AreEqual("tobi zürcher", dir.ReadFile("app.config"));
            }
        }

        [Test]
        public void Transform_WithDeleteFlag_Templates()
        {
            using (var dir = new TestFolder())
            {
                // arrange
                dir.AddFolder(SearchInParentFolderLocator.EnvironmentFolderName);
                dir.AddFile("unit.xml".RelativeTo(SearchInParentFolderLocator.EnvironmentFolderName), GetEnvironment().ToXml());
                dir.AddFile("app.template.config", "${firstname}");

                // act
                Transform("unit", dir.DirectoryInfo.FullName, deleteTemplates: true);

                // assert
                Assert.IsFalse(dir.FileExists("app.template.config"));
                Assert.AreEqual("tobi", dir.ReadFile("app.config"));
            }
        }

        [Test]
        public void Transform_With_Password()
        {
            using (var dir = new TestFolder())
            {
                // arrange
                dir.AddFolder(SearchInParentFolderLocator.EnvironmentFolderName);

                var env = new Environment(
                    "unit",
                    new Variable("var1", "top-secret", true));

                env.EncryptVariables("password");

                dir.AddFile("unit.xml".RelativeTo(SearchInParentFolderLocator.EnvironmentFolderName), env.ToXml());
                dir.AddFile("test.template.config", "${var1}");

                // act
                Transform("unit", dir.DirectoryInfo.FullName, "password");

                // assert
                Assert.IsFalse(dir.ReadFile(Path.Combine(SearchInParentFolderLocator.EnvironmentFolderName, "unit.xml")).Contains("top-secret")); // make sure pw is encrypted
                Assert.AreEqual("top-secret", dir.ReadFile("test.config"));
            }
        }

        [Test]
        public void Transform_With_WrongPassword()
        {
            using (var dir = new TestFolder())
            {
                // arrange
                dir.AddFolder(SearchInParentFolderLocator.EnvironmentFolderName);

                var env = new Environment(
                    "unit",
                    new Variable("var1", "top-secret", true));

                env.EncryptVariables("password");

                dir.AddFile("unit.xml".RelativeTo(SearchInParentFolderLocator.EnvironmentFolderName), env.ToXml());
                dir.AddFile("test.template.config", "${var1}");

                // act
                Transform("unit", dir.DirectoryInfo.FullName, "wrong-password");

                // assert
                Assert.IsFalse(dir.ReadFile(Path.Combine(SearchInParentFolderLocator.EnvironmentFolderName, "unit.xml")).Contains("top-secret")); // make sure pw is encrypted
                Assert.IsTrue(dir.ReadFile("test.config").Contains("Wrong password provided for"));
            }
        }

        [Test]
        public void Transform_With_Correct_PasswordFile()
        {
            using (var dir = new TestFolder())
            {
                // arrange
                dir.AddFolder(SearchInParentFolderLocator.EnvironmentFolderName);

                var env = new Environment(
                    "unit-test",
                    new Variable("var1", "top-secret", true),
                    new Variable("var2", "sesam öffne dich", true),
                    new Variable("var3", "no secret"));

                env.EncryptVariables("very-secret-password");
                dir.AddFile("password.txt", "very-secret-password");

                dir.AddFile("unit.xml".RelativeTo(SearchInParentFolderLocator.EnvironmentFolderName), env.ToXml());
                dir.AddFile("test.template.config", "${var1},${var2},${var3}");

                // act
                Transform("unit", dir.DirectoryInfo.FullName, passwordFile: Path.Combine(dir.DirectoryInfo.FullName, "password.txt"));

                // assert
                Assert.That(dir.ReadFile("test.config"), Is.EqualTo("top-secret,sesam öffne dich,no secret"));
            }
        }

        [Test]
        public void Encrypt_Variables_By_Password()
        {
            using (var dir = new TestFolder())
            {
                // arrange
                var env = new Environment(
                    "unit-test",
                    new Variable("var1", "top-secret", true));

                dir.AddFolder(SearchInParentFolderLocator.EnvironmentFolderName);
                dir.AddFile("unit-test.xml".RelativeTo(SearchInParentFolderLocator.EnvironmentFolderName), env.ToXml());
                
                // act
                EncryptVariables(dir.DirectoryInfo.FullName, "password");

                // assert
                Assert.That(dir.ReadFile("unit-test.xml".RelativeTo(SearchInParentFolderLocator.EnvironmentFolderName)), !Contains.Item("top-secret"));
                Assert.IsNotEmpty(dir.ReadFile("unit-test.xml".RelativeTo(SearchInParentFolderLocator.EnvironmentFolderName)));
            }
        }

        [Test]
        public void Create_Password_File_Generates_Something_Not_Empty()
        {
            using (var dir = new TestFolder())
            {
                // act
                CreatePasswordFile("password.txt".RelativeTo(dir.DirectoryInfo));
                
                Assert.IsFalse(string.IsNullOrEmpty(dir.ReadFile("password.txt")));
            }
        }

        private void Transform(string environmentName = "", string path = "", string password = "", string passwordFile = "", bool deleteTemplates = false)
        {
            var args = new List<string>() { "transform", "--environment=" + environmentName, "--path=" + path, "--password-file=" + passwordFile };

            if (deleteTemplates)
                args.Add("--delete-templates");

            if (!string.IsNullOrEmpty(password))
                args.Add("--password=" + password);

            Program.Main(args.ToArray());
        }

        private void EncryptVariables(string path, string password = "", string passwordFile = "")
        {
            var args = new List<string>()
                       {
                           "encrypt",
                           "--path=" + path,
                           "--password=" + password,
                           "--password-file=" + passwordFile
                       };

            Program.Main(args.ToArray());
        }

        private void CreatePasswordFile(string filename)
        {
            var args = new List<string>() { "create-passwordfile", "--password-file=" + filename };

            Program.Main(args.ToArray());
        }

        private Environment GetEnvironment()
        {
            return new Environment(
                "unit", 
                new Variable("firstname", "tobi"),
                new Variable("lastname", "zürcher"));
        }
    }
}