using System.IO;
using System.Management.Automation;
using Transformer.Core;
using Transformer.Core.Logging;
using Transformer.Core.Template;

namespace Transformer.PowerShell
{
    [Cmdlet(VerbsLifecycle.Invoke, "DirectoryTransform")]
    public class InvokeDirectoryTransform : PSCmdlet
    {
        [Parameter]
        public string Environment { get; set; }

        [Parameter]
        public string SubEnvironment { get; set; }

        [Parameter]
        public string EnvironmentDirectory { get; set; }

        [Parameter]
        public string PasswordFile { get; set; }

        [Parameter]
        public string Password { get; set; }

        [Parameter]
        public bool DeleteTemplates { get; set; }

        [Parameter(Mandatory = true)]
        public string Directory { get; set; }

        public static ILog Log { get; private set; }
    
        protected override void ProcessRecord()
        {
            LogManager.LogFactory = new PowerShellCommandLineLogFactory();
            Log = LogManager.GetLogger(typeof (InvokeDirectoryTransform));

            try
            {
                IEnvironmentFolderLocator locator = null;

                if (!string.IsNullOrEmpty(EnvironmentDirectory))
                {
                    if (!System.IO.Directory.Exists(EnvironmentDirectory))
                    {
                        throw new DirectoryNotFoundException(string.Format("Environment directory {0} not found.", EnvironmentDirectory));
                    }

                    locator = new StaticFolderEnvironmentLocator(EnvironmentDirectory);
                }
                else if (!string.IsNullOrEmpty(Environment))
                {
                    locator = new SearchInParentFolderLocator(Directory);
                }

                var envProvider = new EnvironmentProvider(locator);

                var templateEngine = new TemplateEngine();

                var env = envProvider.GetEnvironment(Environment);

                var aesKey = LoadAesKeyIfProvided();

                if (string.IsNullOrEmpty(aesKey) == false)
                {
                    env.DecryptVariables(aesKey);
                }

                templateEngine.TransformDirectory(Directory, env, SubEnvironment, DeleteTemplates);
            }
            catch (DirectoryNotFoundException)
            {
                Log.Warn(SearchInParentFolderLocator.EnvironmentFolderName + " folder not found for " + Directory + "!");
            }
            catch (FileNotFoundException exception)
            {
                Log.Error(exception.Message);
            }
        }

        private string LoadAesKeyIfProvided()
        {
            var aesKey = string.Empty;

            if (string.IsNullOrEmpty(PasswordFile) == false)
            {
                if (!File.Exists(PasswordFile))
                {
                    Log.WarnFormat("PasswordFile {0} doesnt exist!", PasswordFile);
                }

                try
                {
                    aesKey = File.ReadAllText(PasswordFile);
                }
                catch
                {
                    Log.WarnFormat("Could not read PasswordFile {0}.", PasswordFile);
                }
            }
            else if (string.IsNullOrEmpty(Password) == false)
            {
                aesKey = Password;
            }

            return aesKey;
        }
    }
}