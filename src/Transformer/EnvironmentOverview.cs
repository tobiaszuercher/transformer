using System.Xml.Serialization;

namespace PowerDeploy.Transformer
{
    [XmlRoot("environment", Namespace = "")]
    public class EnvironmentOverview
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }
    }
}