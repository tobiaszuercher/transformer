using CommandLine;

namespace Transformer
{
    public class TransformOptions
    {
        [Option('e', "environment")]
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
}