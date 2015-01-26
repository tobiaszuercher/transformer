using CommandLine;
using CommandLine.Text;

namespace Transformer
{
    public class Options
    {
        [VerbOption("transform", HelpText = "Transforms template files...")]
        public TransformOptions TransformVerb { get; set; }

        //[VerbOption("encrypt", HelpText = "Encrypts...")]
        //public EncryptEnvironmentOptions EncryptEnvrionmentVerb { get; set; }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("<<app title>>", "<<app version>>"),
                Copyright = new CopyrightInfo("<<app author>>", 2014),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("<<license details here.>>");
            help.AddPreOptionsLine("Usage: app -p Someone");
            help.AddOptions(this);
            return help;
        }
    }
}