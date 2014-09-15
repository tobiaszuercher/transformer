using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using PowerDeploy.Transformer;
using Transformer.Model;

namespace Transformer
{
    public class XmlEnvironmentSerializer : IEnvironmentSerializer
    {
        public Environment Deserialize(string file)
        {
            var xml = File.ReadAllText(file);

            var serializer = new XmlSerializer(typeof(Environment));
            var environment = serializer.Deserialize(new XmlTextReader(new StringReader(xml))) as Environment;
            
            // todo: add error handling for failed deserializing
            environment.Variables.Add(new Variable() { Name = "env", Value = environment.Name.ToUpper(CultureInfo.InvariantCulture) });

            return environment;
        }
    }
}