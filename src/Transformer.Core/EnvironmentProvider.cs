using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ServiceStack;
using Environment = Transformer.Core.Model.Environment;

namespace Transformer.Core
{
    public class EnvironmentProvider : IEnvironmentProvider
    {
        private IEnvironmentSerializer Serializer { get; set; }
        private IEnvironmentFolderLocator FolderLocator { get; set; }

        public EnvironmentProvider(IEnvironmentFolderLocator locator)
        {
            Serializer = new XmlEnvironmentSerializer();
            FolderLocator = locator;
        }

        public EnvironmentProvider(string environmentDir)
            : this(new StaticFolderEnvironmentLocator(environmentDir))
        {
        }

        public Environment GetEnvironment(string environmentName)
        {
            var environmentFile = FolderLocator.GetEnvironmentFile("{0}.xml".Fmt(environmentName));

            if (!File.Exists(environmentFile))
            {
                throw new FileNotFoundException("Environment file for environment {0} not found.", environmentName);    
            }

            var environment = Serializer.Deserialize(environmentFile);

            // include other environments if defined
            if (!string.IsNullOrEmpty(environment.Include))
            {
                var envToInclude = environment.Include.Split(',');

                foreach (var toInclude in envToInclude)
                {
                    var includeEnv = Serializer.Deserialize(FolderLocator.GetEnvironmentFile(toInclude));

                    // just add the variables which are not available in the requested environment
                    foreach (var variable in includeEnv.Variables)
                    {
                        if (environment.Variables.All(v => !v.Name.Equals(variable.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            environment.Variables.Add(variable);
                        }
                    }
                }
            }

            return environment;
        }

        public IEnumerable<string> GetEnvironments(bool excludeShared = true)
        {
            return Directory.GetFiles(FolderLocator.EnvironmentFolder, "*.xml");
        }
    }
}