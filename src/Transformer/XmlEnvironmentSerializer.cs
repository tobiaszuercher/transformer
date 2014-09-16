using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using PowerDeploy.Transformer.Logging;
using Transformer.Logging;
using Transformer.Model;

namespace Transformer
{
    public class XmlEnvironmentSerializer : IEnvironmentSerializer
    {
        private static ILog _log = LogManager.GetLogger(typeof (XmlEnvironmentSerializer));

        public Environment Deserialize(string file)
        {
            _log.DebugFormat("Deserializing environment file {0}", file);

            var xml = File.ReadAllText(file);
            
            var serializer = new XmlSerializer(typeof(Environment));
            var environment = serializer.Deserialize(new XmlTextReader(new StringReader(xml))) as Environment;
            environment.Name = Path.GetFileNameWithoutExtension(file);
           
            environment.Variables.Add(new Variable() { Name = "env", Value = environment.Name.ToUpper(CultureInfo.InvariantCulture) });

            return environment;
        }
    }
}