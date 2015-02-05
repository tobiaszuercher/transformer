using System;

namespace Transformer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new Options();

            if (!CommandLine.Parser.Default.ParseArguments(args, options,
                (verb, subOptions) =>
                {
                    if (verb == "transform")
                    {
                        var commandOptions = (TransformOptions) subOptions;

                        Commands.Transform(
                            commandOptions.Environment,
                            commandOptions.SubEnvironment,
                            commandOptions.Path,
                            commandOptions.DeleteTemplates,
                            commandOptions.EnvironmentPath,
                            commandOptions.PasswordFile,
                            commandOptions.Password);
                    }
                    else if (verb == "create-passwordfile")
                    {
                        var commandOptions = (CreatePasswordFileOptions) subOptions;

                        Commands.CreatePasswordFile(commandOptions.PasswordFile);
                    }
                }))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            };
        }
    }
}
