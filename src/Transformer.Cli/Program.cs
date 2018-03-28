using System;
using System.Collections.Generic;
using CommandLine;

namespace Transformer.Cli
{
    public class Program
    {
         public static int Main(string[] args) {
            return Parser.Default.ParseArguments<TransformOptions, EncryptOptions, ListOptions>(args)
                .MapResult(
                    (TransformOptions opts) => Transform(opts),
                    (EncryptOptions opts) => Encrypt(opts),
                    (ListOptions opts) => ListEnvironments(opts),
                    errs => 1);
        }

        private static int ListEnvironments(ListOptions opts)
        {
            Commands.List(Environment.CurrentDirectory);

            return 1337;
        }

        private static  int Encrypt(EncryptOptions opts)
        {
            throw new NotImplementedException();
        }

        private static int Transform(TransformOptions opts)
        {
            throw new NotImplementedException();
        }
    }
}
