using CommandLine;

namespace Transformer.Cli
{
    [Verb("encrypt", HelpText = "Encrypt variables")]
    public class EncryptOptions
    {
        [Option("path")]
        public string Path { get; set; }

        [Option('p', "password")]
        public string Password { get; set; }

        [Option('f', "password-file")]
        public string PasswordFile { get; set; }
    }
}