using CommandLine;

namespace Transformer.Cli
{
    [Verb("change-password")]
    public class ChangePasswordOptions
    {
        [Option('o', "old-password")]
        public string OldPassword { get; set; }

        [Option('n', "new-password")]
        public string NewPassword { get; set; }

        [Option('p', "path")]
        public string Path { get; set; }
    }
}