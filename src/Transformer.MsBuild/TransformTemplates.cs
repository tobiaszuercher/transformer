using System.IO;
using Microsoft.Build.Utilities;
using PowerDeploy.MsBuild;
using Transformer.Core;
using Transformer.Core.Logging;
using Transformer.Core.Template;

namespace Transformer.MsBuild
{
    public class TransformTemplates : Task
    {
        public string Environment { get; set; }
        public string SubEnvironment { get; set; }
        public string Directory { get; set; }

        public override bool Execute()
        {
            LogManager.LogFactory = new BuildLogFactory(Log);

            Log.LogCommandLine("Transform all project files within the current solution...");
            
            try
            {
                var envProvider = new EnvironmentProvider(new StaticFolderEnvironmentLocator(Directory));

                var templateEngine = new TemplateEngine();
                templateEngine.TransformDirectory(Directory, envProvider.GetEnvironment(Environment), SubEnvironment);
            }
            catch (DirectoryNotFoundException)
            {
                Log.LogError(SearchInParentFolderLocator.EnvironmentFolderName + " folder not found for project " + Directory + "! :(");
                return false;
            }
            catch (FileNotFoundException exception)
            {
                Log.LogError(exception.Message);
                return false;
            }

            return true;
        }
    }
}
