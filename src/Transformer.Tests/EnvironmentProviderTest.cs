using System.Collections.Generic;
using System.IO;
using Transformer;
using Transformer.Core;
using Transformer.Model;
using Xunit;
using Environment = System.Environment;

namespace Transformer.Tests
{
    public class EnvironmentProviderTest
    {
        [Fact]
        public void Find_Environment()
        {
            using (var workDir = new TestFolder())
            {
                workDir.AddFolder(SearchInParentFolderLocator.EnvironmentFolderName);
                workDir.AddFile(SearchInParentFolderLocator.EnvironmentFolderName + "/unittest.xml", new Model.Environment()
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

                Assert.NotNull(result);
                Assert.Equal("unittest", result.Name);
                Assert.Equal("Jack", result["Firstname"].Value);
                Assert.Equal("Bauer", result["Lastname"].Value);
            }
        }

        ////[Fact]
        ////[ExpectedException(typeof(DirectoryNotFoundException))]
        ////public void Find_Environment_With_Dir_Not_Existing()
        ////{
        ////    // TODO: use Environment.GetEnvironmentVariable("LocalAppData");
        ////    using (var workDir = new TestFolder(Environment.SpecialFolder.LocalApplicationData))
        ////    {
        ////        workDir.AddFolder("dir1");
        ////        workDir.AddFolder("dir1/subdir1");

        ////        var target = new EnvironmentProvider(new SearchInParentFolderLocator(Path.Combine(workDir.DirectoryInfo.FullName, "dir1/subdir1")));
        ////        target.GetEnvironment("unittest");
        ////    }
        ////}

        ////[Test]
        ////[ExpectedException(typeof(FileNotFoundException))]
        ////public void Find_Non_Existing_Environment()
        ////{
        ////    using (var workDir = new TestFolder(Environment.SpecialFolder.LocalApplicationData))
        ////    {
        ////        workDir.AddFolder("dir1");
        ////        workDir.AddFolder(SearchInParentFolderLocator.EnvironmentFolderName);
        ////        workDir.AddFolder("dir1/subdir1");

        ////        var target = new EnvironmentProvider(Path.Combine(workDir.DirectoryInfo.FullName, "dir1/subdir1"));
        ////        target.GetEnvironment("unittest");
        ////    }
        ////}

        ////[Fact]
        ////public void Find_Environment_In_Same_Dir()
        ////{
        ////    const string environmentXml = @"<?xml version=""1.0""?>
        ////                <environment description=""Used for unit tests, not a real environment"">
        ////                  <variable name=""Name"" value=""Tobi"" />
        ////                  <variable name=""Jack"" value=""Bauer"" />
        ////                </environment>";

        ////    using (var workDir = new TestFolder(Environment.SpecialFolder.LocalApplicationData))
        ////    {
        ////        workDir.AddFolder("dir1");
        ////        workDir.AddFolder("dir1/subdir1");
        ////        workDir.AddFolder(SearchInParentFolderLocator.EnvironmentFolderName);
        ////        workDir.AddFile(SearchInParentFolderLocator.EnvironmentFolderName + "/unittest.xml", environmentXml);

        ////        var target = new EnvironmentProvider(new SearchInParentFolderLocator(workDir.DirectoryInfo.FullName));
        ////        var result = target.GetEnvironment("unittest");

        ////        Assert.Equal("unittest", result.Name);
        ////        Assert.Equal("Tobi", result["Name"].Value);
        ////        Assert.Equal("Bauer", result["Jack"].Value);
        ////    }
        ////}
    }
}