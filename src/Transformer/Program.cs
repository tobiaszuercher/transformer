using System;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
using Transformer.Core.Logging;
using LogManager = NLog.LogManager;

namespace Transformer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new Options();
            
            var logConfig = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget() { Layout = @"${message}" };

            logConfig.AddTarget("console", consoleTarget);

            LogManager.Configuration = logConfig;
            
            Core.Logging.LogManager.LogFactory = new NLogFactory();

            if (!CommandLine.Parser.Default.ParseArguments(args, options,
                (verb, subOptions) =>
                {
                    logConfig.LoggingRules.Add(new LoggingRule("*", ((OptionsBase)subOptions).Verbose ? LogLevel.Debug : LogLevel.Info, consoleTarget));

                    if (verb == "transform")
                    {
                        var commandOptions = (TransformOptions) subOptions;
                        
                        if (commandOptions == null)
                        {
                            Console.WriteLine("Example usage: transformer.exe transform --environment=test --path=c:\\temp");
                            Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                        }

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
                    else if (verb == "encrypt")
                    {
                        var commandOptions = (EncryptOptions) subOptions;

                        Commands.EncryptVariables(commandOptions.Path, commandOptions.Password, commandOptions.PasswordFile);
                    }
                    else if (verb == "list")
                    {
                        Commands.List(Environment.CurrentDirectory);
                    }
                }))
            {
                //Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            };

            LogManager.GetLogger("Bla").Info("Info message");
            LogManager.GetLogger("Bla").Debug("Debug message");
        }
    }
}
