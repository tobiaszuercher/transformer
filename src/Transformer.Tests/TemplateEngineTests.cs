using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PowerDeploy.Transformer;
using PowerDeploy.Transformer.Template;

namespace Transformer.Tests
{
    [TestFixture]
    public class TemplateEngineTests
    {
        [Test]
        public void Transform_Package_Test()
        {
            var mock = new Mock<IEnvironmentSerializer>();
            mock.Setup(provider => provider.Deserialize("unit")).Returns(GetUnitEnvironment());

            var target = new TemplateEngine();
            ////target.ConfigurePackage(@"c:\temp\nuget\Testpackage.1.0.0.nupkg", "DEV", @"c:\temp\");
            /// // TODO:
        }

        [Test]
        public void Transform_Read_Only_File()
        {
            var mock = new Mock<IEnvironmentSerializer>();
            mock.Setup(provider => provider.Deserialize("unit")).Returns(GetUnitEnvironment());

            var target = new TemplateEngine();
            using (var dir = new TestFolder())
            {
                dir.AddFile("read-only.template.txt", "whatever: ${var1}");
                dir.AddFile("read-only.txt", "will be transformed").SetReadOnly();

                // before the bugfix: this threw a Exception because the file was ReadOnly
                target.TransformDirectory(dir.DirectoryInfo.FullName, GetUnitEnvironment());

                Assert.AreNotEqual("will be transformed", dir.ReadFile("read-only.txt"));
            }
        }

        [Test]
        public void Sub_Environments_Are_Parsed()
        {
            var mock = new Mock<IEnvironmentSerializer>();
            mock.Setup(provider => provider.Deserialize("unit")).Returns(GetUnitEnvironment());

            var target = new TemplateEngine();

            using (var dir = new TestFolder())
            {
                dir.AddFile("app.template.config", "subenv: ${subenv}");

                target.TransformDirectory(dir.DirectoryInfo.FullName, GetUnitEnvironment(), "_bugfix1337");

                Assert.AreEqual("subenv: _bugfix1337", dir.ReadFile("app.config"));
            }
        }

        private Environment GetUnitEnvironment()
        {
            return new Environment()
            {
                Name = "Unit",
                Description = "UnitTest",
                Variables = new List<Variable>()
                {
                    new Variable() { Name = "var1", Value = "Val1" }, 
                    new Variable() { Name = "var2", Value = "Val2" }, 
                    new Variable() { Name = "ar3", Value = "Val3" },
                }
            };
        }
    }
}