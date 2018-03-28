using CommandLine;

namespace Transformer.Cli
{
    [Verb("create-passwordfile")]
    public class CreatePasswordFileOptions
    {
        [Option('f', "password-file")] 
        public string PasswordFile { get; set; }
    }
}