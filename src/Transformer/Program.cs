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

                        Console.WriteLine("transform");
                    }
                }))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            };
        }
    }
}
