using System.IO;

namespace Transformer
{
    public class StaticFolderEnvironmentLocator : IEnvironmentFolderLocator
    {
        public string EnvironmentFolder { get; private set; }

        public StaticFolderEnvironmentLocator(string dir)
        {
            EnvironmentFolder = dir;
        }

        public virtual string GetEnvironmentFile(string environmentName)
        {
            return Path.Combine(EnvironmentFolder, environmentName);
        }
    }
}