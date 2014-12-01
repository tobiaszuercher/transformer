using CommandLine;

namespace Transformer
{
    public class EncryptEnvironmentOptions
    {
        [Option('e', "enviornment")]
        public string Environment { get; set; } 
    }
}