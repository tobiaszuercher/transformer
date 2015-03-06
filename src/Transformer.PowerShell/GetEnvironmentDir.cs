using System.Management.Automation;
using Transformer.Core;
using Transformer.Core.Logging;

namespace Transformer.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "EnvironmentDir")]
    public class GetEnvironmentDir : PSCmdlet
    {
        [Parameter(Mandatory = false, Position = 1)]
        public string Directory { get; set; }

        protected override void ProcessRecord()
        {
            LogManager.LogFactory = new PowerShellCommandLineLogFactory();

            if (string.IsNullOrEmpty(Directory))
            {
                Directory = SessionState.Path.CurrentLocation.Path;
            }

            var environmentDirectory = new SearchInParentFolderLocator(Directory).EnvironmentFolder;

            WriteObject(environmentDirectory);
        }
    }
}