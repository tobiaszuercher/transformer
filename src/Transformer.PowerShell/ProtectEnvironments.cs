using System.IO;
using System.Management.Automation;
using PowerDeploy.Transformer.Logging;
using Transformer.Core;
using Transformer.Core.Logging;

namespace Transformer.PowerShell
{
    /// <summary>
    /// Encrypts variable values, removes the
    /// </summary>
    [Cmdlet(VerbsSecurity.Protect, "Environments")]
    public class ProtectEnvironments : Cmdlet
    {
        [Parameter]
        public string PasswordFile { get; set; }

        [Parameter]
        public string Password { get; set; }

        [Parameter(Mandatory = true)]
        public string Directory { get; set; }

        public static ILog Log { get; private set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            LogManager.LogFactory = new PowerShellCommandLineLogFactory();
            Log = LogManager.GetLogger(typeof(InvokeDirectoryTransform));

            Log.Info("Encrypting all variables marked with do-encrypt attribute");

            string aesKey = string.Empty;

            if (string.IsNullOrEmpty(PasswordFile) == false)
            {
                if (File.Exists(PasswordFile) == false)
                {
                    Log.ErrorFormat("Passwordfile '{0}' doesnt exist! Abording...", PasswordFile);
                    return;
                }

                aesKey = File.ReadAllText(PasswordFile);

                if (string.IsNullOrEmpty(aesKey))
                {
                    Log.ErrorFormat("Passwordfile '{0}' is empty! Abording...", PasswordFile);
                    return;
                }
            }
            else if (string.IsNullOrEmpty(Password) == false)
            {
                aesKey = Password;
            }
            else
            {
                Log.Error("Please provide a Password or PasswordFile");
                return;
            }

            var environmentEncrypter = new EnvironmentEncrypter(new EnvironmentProvider(new StaticFolderEnvironmentLocator(Directory)), aesKey);
            environmentEncrypter.EncryptAllEnvironments();
        }
    }
}