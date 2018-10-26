using System;
using CommandLine;
using ServiceStack.Text;
using Transformer.Logging;

namespace Transformer.Cli
{
    public class Program
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public static int Main(string[] args)
        {
            LogProvider.SetCurrentLogProvider(new ColoredConsoleLogProvider());

            Log.Info("Gugus");

            return Parser.Default.ParseArguments<
                    TransformOptions,
                    EncryptOptions,
                    ListOptions,
                    ChangePasswordOptions,
                    CreatePasswordFileOptions>(args)
                .MapResult(
                    (TransformOptions opts) => Transform(opts),
                    (EncryptOptions opts) => Encrypt(opts),
                    (ListOptions opts) => ListEnvironments(opts),
                    (ChangePasswordOptions opts) => ChangePassword(opts),
                    (CreatePasswordFileOptions opts) => Commands.CreatePasswordFile(opts.PasswordFile),
                    errs => 1);
        }

        private static int ChangePassword(ChangePasswordOptions opts)
        {
            Commands.ChangePassword(opts.Path, opts.OldPassword, opts.NewPassword);

            return 1337;
        }

        private static int ListEnvironments(ListOptions opts)
        {
            Commands.List(Environment.CurrentDirectory);

            return 1337;
        }

        private static int Encrypt(EncryptOptions opts)
        {
            Commands.EncryptVariables(opts.Path, opts.Password, opts.PasswordFile);

            return 1337;
        }

        private static int Transform(TransformOptions opts)
        {
            Commands.Transform(
                opts.Environment,
                opts.SubEnvironment,
                opts.Path,
                opts.DeleteTemplates,
                opts.EnvironmentPath,
                opts.PasswordFile,
                opts.Password);

            return 0;
        }
    }
}
