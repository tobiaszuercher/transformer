using CommandLine;

namespace Transformer
{
    public class CreatePasswordFileOptions : OptionsBase
    {
        [Option('f', "password-file")] 
        public string PasswordFile { get; set; }
    }
}