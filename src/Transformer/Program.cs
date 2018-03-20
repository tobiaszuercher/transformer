using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using Transformer.Core.Logging;

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

            NLog.LogManager.Configuration = logConfig;
            
            LogManager.LogFactory = new NLogFactory();

            if (!CommandLine.Parser.Default.ParseArguments(args, options,
                (verb, subOptions) =>
                {
                    if (subOptions != null)
                        logConfig.LoggingRules.Add(new LoggingRule("*", ((OptionsBase)subOptions).Verbose ? NLog.LogLevel.Debug : NLog.LogLevel.Info, consoleTarget));

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
                        var commandOptions = (CreatePasswordFileOptions)subOptions;

                        Commands.CreatePasswordFile(commandOptions.PasswordFile);
                    }
                    else if (verb == "encrypt")
                    {
                        var commandOptions = (EncryptOptions) subOptions;

                        Commands.EncryptVariables(commandOptions.Path, commandOptions.Password, commandOptions.PasswordFile);
                    }
                    else if (verb == "change-password")
                    {
                        var commandOptions = (ChangePasswordOptions)subOptions;

                        Commands.ChangePassword(commandOptions.Path, commandOptions.OldPassword, commandOptions.NewPassword);
                    }
                    else if (verb == "list")
                    {
                        Commands.List(Environment.CurrentDirectory);
                    }
                }))
            {
                //Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            };
        }
    }
}
