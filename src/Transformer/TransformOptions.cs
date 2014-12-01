using CommandLine;

namespace Transformer
{
    public class Options
    {
        [VerbOption("transform", HelpText = "Transforms template files...")]
        public TransformOptions TransformVerb { get; set; }

        [VerbOption("encrypt", HelpText = "Encrypts...")]
        public EncryptEnvironmentOptions EncryptEnvrionmentVerb { get; set; }
    }

    public class TransformOptions
    {
        [Option('e', "enviornment")]
        public string Environment { get; set; } 
    }
}