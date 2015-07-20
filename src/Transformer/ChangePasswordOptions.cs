using CommandLine;

namespace Transformer
{
    public class ChangePasswordOptions : OptionsBase
    {
        [Option('o', "old-password")]
        public string OldPassword { get; set; }

        [Option('n', "new-password")]
        public string NewPassword { get; set; }

        [Option('p', "path")]
        public string Path { get; set; }
    }
}