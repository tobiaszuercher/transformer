using CommandLine;

namespace Transformer
{
    public class CreatePasswordFileOptions
    {
        [Option('f', "password-file")] 
        public string PasswordFile { get; set; }
    }
}