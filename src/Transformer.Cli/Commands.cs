using System;
using System.IO;
using System.Linq;
using Transformer.Cryptography;
using Transformer.Logging;
using Transformer.Template;

namespace Transformer.Cli
{
    public class Commands
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public static int Transform(
            string environmentName, 
            string subEnvironmentName, 
            string path, 
            bool deleteTemplates, 
            string environmentDirectory = null, 
            string passwordFile = null, 
            string password = null)
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

                return 1;
            }
            catch (ArgumentException)
            {
                Log.Warn("Provided path is invalid.");

                return 1;
            }
            catch (FileNotFoundException exception)
            {
                Log.Error(exception.Message);

                return 1;
            }

            return 0;
        }

        public static int CreatePasswordFile(string filename)
        {
            try
            {
                var password = RandomStringProvider.Provide(64); // TODO: this uses System.Web. Check if there is a better way to generate the pw.
                var targetPath = Path.GetFullPath(filename);
                
                File.WriteAllText(targetPath, password);

                Log.Info($"Created password in file {targetPath}");
            }
            catch (Exception e)
            {
                Log.ErrorException("Error creating password file", e);

                return 1;
            }

            return 0;
        }

        public static void EncryptVariables(string path, string password = "", string passwordFile = "")
        {
            Log.Debug("Encrypting all variables marked with do-encrypt attribute");

            string aesKey = string.Empty;

            if (string.IsNullOrEmpty(passwordFile) == false)
            {
                passwordFile = Path.GetFullPath(passwordFile);

                if (File.Exists(passwordFile) == false)
                {
                    // TODO: loggin
                    //Log.ErrorFormat("Password file '{0}' doesnt exist! Abording...", passwordFile);
                    return;
                }

                aesKey = File.ReadAllText(passwordFile);

                if (string.IsNullOrEmpty(aesKey))
                {
                    // TODO
                    // Log.ErrorFormat("Password file '{0}' is empty! Abording...", passwordFile);
                    return;
                }
            }
            else if (string.IsNullOrEmpty(password) == false)
            {
                aesKey = password;
            }
            else
            {
                Log.Error("Please provide a Password or PasswordFile");
                return;
            }

            var environmentEncrypter = new EnvironmentEncrypter(new EnvironmentProvider(new SearchInParentFolderLocator(path)), aesKey);
            environmentEncrypter.EncryptAllEnvironments();
        }

        public static void ChangePassword(string path, string oldPassword, string newPassword)
        {
            Log.Debug("Changing password for encrypted variables.");

            var encrypter = new EnvironmentEncrypter(new EnvironmentProvider(new SearchInParentFolderLocator(path)), oldPassword);
            encrypter.ChangeEncryptionKeyInAllEnvironments(newPassword);
        }

        private static string LoadAesKeyIfProvided(string password, string passwordFile = null)
        {
            var aesKey = string.Empty;

            if (string.IsNullOrEmpty(passwordFile) == false)
            {
                if (!File.Exists(passwordFile))
                {
                    // TODO
                    // Log.WarnFormat("PasswordFile {0} doesnt exist!", passwordFile);
                }

                try
                {
                    aesKey = File.ReadAllText(passwordFile);
                }
                catch
                {
                    // TODO
                    // Log.ErrorFormat("Could not read PasswordFile {0}.", passwordFile);
                }
            }
            else if (string.IsNullOrEmpty(password) == false)
            {
                aesKey = password;
            }

            return aesKey;
        }

        public static void List(string currentDirectory)
        {
            //LogManager.LogFactory.DisableLogging();

            var locator = new SearchInParentFolderLocator(currentDirectory);

            foreach (var file in Directory.EnumerateFiles(locator.EnvironmentFolder, "*.xml").Where(f => !f.StartsWith("_")))
            {
                Console.WriteLine(new FileInfo(file).Name.Replace(".xml", string.Empty));
            }

            //LogManager.LogFactory.EnableLogging();
        }
    }
}