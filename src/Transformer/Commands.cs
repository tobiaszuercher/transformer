using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Security;
using Transformer.Core;
using Transformer.Core.Logging;
using Transformer.Core.Template;

namespace Transformer
{
    public class Commands
    {
        private static ILog Log = LogManager.GetLogger(typeof (Commands));

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
            catch (ArgumentException)
            {
                Log.Warn("Provided path is invalid.");

                return false;
            }
            catch (FileNotFoundException exception)
            {
                Log.Error(exception.Message);

                return false;
            }

            return true;
        }

        public static void CreatePasswordFile(string filename)
        {
            var password = Membership.GeneratePassword(64, 0); // TODO: this uses System.Web. Check if there is a better way to generate the pw.
            var targetPath = Path.GetFullPath(filename);
            
            File.WriteAllText(targetPath, password);

            Log.InfoFormat("Created password in file {0}", targetPath);
        }

        public static void EncryptVariables(string path, string password = "", string passwordFile = "")
        {
            Log.Info("Encrypting all variables marked with do-encrypt attribute");

            string aesKey = string.Empty;

            if (string.IsNullOrEmpty(passwordFile) == false)
            {
                passwordFile = Path.GetFullPath(passwordFile);

                if (File.Exists(passwordFile) == false)
                {
                    Log.ErrorFormat("Password file '{0}' doesnt exist! Abording...", passwordFile);
                    return;
                }

                aesKey = File.ReadAllText(passwordFile);

                if (string.IsNullOrEmpty(aesKey))
                {
                    Log.ErrorFormat("Password file '{0}' is empty! Abording...", passwordFile);
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

        private static string LoadAesKeyIfProvided(string password, string passwordFile = null)
        {
            var aesKey = string.Empty;

            if (string.IsNullOrEmpty(passwordFile) == false)
            {
                if (!File.Exists(passwordFile))
                {
                    Log.WarnFormat("PasswordFile {0} doesnt exist!", passwordFile);
                }

                try
                {
                    aesKey = File.ReadAllText(passwordFile);
                }
                catch
                {
                    Log.ErrorFormat("Could not read PasswordFile {0}.", passwordFile);
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
            LogManager.LogFactory.DisableLogging();

            var locator = new SearchInParentFolderLocator(currentDirectory);

            foreach (var file in Directory.EnumerateFiles(locator.EnvironmentFolder, "*.xml").Where(f => !f.StartsWith("_")))
            {
                Console.WriteLine(new FileInfo(file).Name.Replace(".xml", string.Empty));
            }

            LogManager.LogFactory.EnableLogging();
        }
    }
}