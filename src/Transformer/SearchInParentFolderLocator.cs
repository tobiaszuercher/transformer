using System.IO;
using System.Linq;
using Transformer.Logging;

namespace Transformer
{
    public class SearchInParentFolderLocator : StaticFolderEnvironmentLocator
    {
        private static ILog Log = LogManager.GetLogger(typeof (SearchInParentFolderLocator));
        public const string EnvironmentFolderName = ".transformer";
            
        public SearchInParentFolderLocator(string startDir)
            : base(FindEnvironmentFolder(startDir).FullName)
        {
        }

        private static DirectoryInfo FindEnvironmentFolder(string startFolder)
        {
            Log.Debug("Look for environment starting in " + startFolder);
            
            var dirInfo = new DirectoryInfo(startFolder);
            var root = Directory.GetDirectoryRoot(startFolder);

            while (dirInfo.FullName != root)
            {
                if (dirInfo.GetDirectories(EnvironmentFolderName).Any())
                {
                    break;
                }

                if (dirInfo.Name == EnvironmentFolderName)
                {
                    dirInfo = dirInfo.Parent;
                    break;
                }

                dirInfo = Directory.GetParent(dirInfo.FullName);
            }

            var envFolder = new DirectoryInfo(Path.Combine(dirInfo.FullName, EnvironmentFolderName));

            if (!envFolder.Exists)
            {
                Log.DebugFormat("No folder found while looking in the parent folders starting at {0}", startFolder);
                throw new DirectoryNotFoundException(EnvironmentFolderName + " folder not found with start folder " + startFolder);
            }

            Log.Debug("Using environment folder from " + envFolder.FullName);

            return envFolder;
        }
    }
}