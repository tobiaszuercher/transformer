using System.IO;
using System.Linq;
using PowerDeploy.Transformer.Logging;
using Transformer.Logging;

namespace Transformer
{
    public class SearchInParentFolderLocator : StaticFolderEnvironmentLocator
    {
        private static ILog Log = LogManager.GetLogger(typeof (SearchInParentFolderLocator));

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
                if (dirInfo.GetDirectories(".powerdeploy").Any())
                {
                    break;
                }

                if (dirInfo.Name == ".powerdeploy")
                {
                    dirInfo = dirInfo.Parent;
                    break;
                }

                dirInfo = Directory.GetParent(dirInfo.FullName);
            }

            var envFolder = new DirectoryInfo(Path.Combine(dirInfo.FullName, ".powerdeploy"));

            if (!envFolder.Exists)
            {
                Log.Warn("Folder " + envFolder.FullName + " is missing!");
                throw new DirectoryNotFoundException(".powerdeploy folder not found with start folder " + startFolder);
            }

            Log.Debug("Using environment folder from " + envFolder.FullName);

            return envFolder;
        }
    }
}