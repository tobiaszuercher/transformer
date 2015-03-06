using CommandLine;

namespace Transformer
{
    public class TransformOptions : OptionsBase
    {
        [Option('e', "environment", Required = true)]
        public string Environment { get; set; }

        [Option('s', "sub-environment")]
        public string SubEnvironment { get; set; }

        [Option("delete-templates")]
        public bool DeleteTemplates { get; set; }

        [Option("path")]
        public string Path { get; set; }

        [Option('p', "password")]
        public string Password { get; set; }

        [Option("password-file")]
        public string PasswordFile { get; set; }

        [Option("environment-path")]
        public string EnvironmentPath { get; set; }
    }

    public abstract class OptionsBase
    {
        [Option('v', "verbose")]
        public bool Verbose { get; set; }
    }
}