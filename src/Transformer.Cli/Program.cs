using System;
using System.Collections.Generic;
using CommandLine;
using ServiceStack.Text;
using Console = Colorful.Console;

namespace Transformer.Cli
{
    public class Program
    {
         public static int Main(string[] args) {
            var bla = Parser.Default.ParseArguments<TransformOptions, EncryptOptions, ListOptions>(args)
                .MapResult(
                    (TransformOptions opts) => Transform(opts),
                    (EncryptOptions opts) => Encrypt(opts),
                    (ListOptions opts) => ListEnvironments(opts),
                    errs => 1);

             bla.PrintDump();
             return 0;
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
                opts.Password,
                opts.PasswordFile);

            return 0;
        }
    }
}
