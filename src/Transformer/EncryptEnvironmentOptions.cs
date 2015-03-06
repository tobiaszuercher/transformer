using CommandLine;

namespace Transformer
{
    public class EncryptEnvironmentOptions
    {
        [Option('e', "enviornment")]
        public string Environment { get; set; }

        [Option('p', "path")]
        public string Path { get; set; }
    }
}