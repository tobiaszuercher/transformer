using System.IO;
using NLog;
using Transformer.Core;
using Transformer.Core.Template;

namespace Transformer
{
    public class Commands
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public static bool Transform(string environmentName, string subEnvironmentName, string path, bool deleteTemplates, string environmentDirectory = null, string passwordFile = null, string password = null)
        {
            try
            {
                IEnvironmentFolderLocator locator = null;

                if (!string.IsNullOrEmpty(environmentDirectory))
                {
                    if (!Directory.Exists(environmentDirectory))
                    {
                        throw new DirectoryNotFoundException(string.Format("Environment directory {0} not found.", environmentDirectory));
                    }

                    locator = new StaticFolderEnvironmentLocator(environmentDirectory);
                }
                else if (!string.IsNullOrEmpty(environmentName))
                {
                    locator = new SearchInParentFolderLocator(path);
                }

                var envProvider = new EnvironmentProvider(locator);
                var templateEngine = new TemplateEngine();
                var env = envProvider.GetEnvironment(environmentName);
                var aesKey = LoadAesKeyIfProvided(password, passwordFile);

                if (string.IsNullOrEmpty(aesKey) == false)
                {
                    env.DecryptVariables(aesKey);
                }

                templateEngine.TransformDirectory(path, env, subEnvironmentName, deleteTemplates);
            }
            catch (DirectoryNotFoundException)
            {
                Log.Warn(SearchInParentFolderLocator.EnvironmentFolderName + " folder not found for " + path + "!");

                return false;
            }
            catch (FileNotFoundException exception)
            {
                Log.Error(exception.Message);

                return false;
            }

            return true;
        }

        private static string LoadAesKeyIfProvided(string password, string passwordFile = null)
        {
            var aesKey = string.Empty;

            if (string.IsNullOrEmpty(passwordFile) == false)
            {
                if (!File.Exists(passwordFile))
                {
                    Log.Warn("PasswordFile {0} doesnt exist!", passwordFile);
                }

                try
                {
                    aesKey = File.ReadAllText(passwordFile);
                }
                catch
                {
                    Log.Error("Could not read PasswordFile {0}.", passwordFile);
                }
            }
            else if (string.IsNullOrEmpty(password) == false)
            {
                aesKey = password;
            }

            return aesKey;
        }
    }
}