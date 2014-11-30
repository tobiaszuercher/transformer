using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Transformer.Core;
using Transformer.Core.Model;
using Environment = System.Environment;

namespace Transformer.Tests
{
    [TestFixture]
    public class EnvironmentProviderTest
    {
        [Test]
        public void Find_Environment()
        {
            using (var workDir = new TestFolder())
            {
                workDir.AddFolder(".powerdeploy");
                workDir.AddFile(".powerdeploy/unittest.xml", new Core.Model.Environment()
                {
                    Name = "unittest",
                    Variables = new List<Variable>()
                    { 
                        new Variable("Firstname", "Jack"), 
                        new Variable("Lastname", "Bauer") 
                    }
                }.ToXml());

                var target = new EnvironmentProvider(new SearchInParentFolderLocator(workDir.DirectoryInfo.FullName));
     
                var result = target.GetEnvironment("unittest");

                Assert.IsNotNull(result);
                Assert.AreEqual("unittest", result.Name);
                Assert.AreEqual("Jack", result["Firstname"].Value);
                Assert.AreEqual("Bauer", result["Lastname"].Value);
            }
        }

        [Test]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void Find_Environment_With_Dir_Not_Existing()
        {
            using (var workDir = new TestFolder(Environment.SpecialFolder.LocalApplicationData))
            {
                workDir.AddFolder("dir1");
                workDir.AddFolder("dir1/subdir1");

                var target = new EnvironmentProvider(new SearchInParentFolderLocator(Path.Combine(workDir.DirectoryInfo.FullName, "dir1/subdir1")));
                target.GetEnvironment("unittest");
            }
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Find_Non_Existing_Environment()
        {
            using (var workDir = new TestFolder(Environment.SpecialFolder.LocalApplicationData))
            {
                workDir.AddFolder("dir1");
                workDir.AddFolder(".powerdeploy");
                workDir.AddFolder("dir1/subdir1");

                var target = new EnvironmentProvider(Path.Combine(workDir.DirectoryInfo.FullName, "dir1/subdir1"));
                target.GetEnvironment("unittest");
            }
        }

        [Test]
        public void Find_Environment_In_Same_Dir()
        {
            const string environmentXml = @"<?xml version=""1.0""?>
                        <environment description=""Used for unit tests, not a real environment"">
                          <variable name=""Name"" value=""Tobi"" />
                          <variable name=""Jack"" value=""Bauer"" />
                        </environment>";

            using (var workDir = new TestFolder(Environment.SpecialFolder.LocalApplicationData))
            {
                workDir.AddFolder("dir1");
                workDir.AddFolder("dir1/subdir1");
                workDir.AddFolder(".powerdeploy");
                workDir.AddFile(".powerdeploy/unittest.xml", environmentXml);

                var target = new EnvironmentProvider(new SearchInParentFolderLocator(workDir.DirectoryInfo.FullName));
                var result = target.GetEnvironment("unittest");

                Assert.AreEqual("unittest", result.Name);
                Assert.AreEqual("Tobi", result["Name"].Value);
                Assert.AreEqual("Bauer", result["Jack"].Value);
            }
        }
    }
}