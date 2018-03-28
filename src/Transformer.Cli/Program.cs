using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
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
                    (ChangePasswordOptions opts) => ChangePassword(opts),
                    (CreatePasswordFileOptions opts) => CreatePasswordFile(opts),
                    errs =>
                    {
                        Console.WriteLine(errs.Dump());
                        
                        return 1;
                    });

             bla.PrintDump();
             return 0;
         }

        private static int CreatePasswordFile(CreatePasswordFileOptions opts)
        {
            Commands.CreatePasswordFile(opts.PasswordFile);
            
            return 1337;
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
                opts.Password,
                opts.PasswordFile);

            return 0;
        }
    }
}
