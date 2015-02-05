using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using PowerDeploy.Transformer.Logging;
using Transformer.Core.Logging;

namespace Transformer.Core.Model
{
    [XmlRoot("environment", Namespace = "")]
    public class Environment
    {
        [XmlAttribute("name")]
        public string Name { get; set; }     

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("include")]
        public string Include { get; set; }

        [XmlElement("variable")]
        public List<Variable> Variables { get; set; }

        [XmlIgnore]
        public Variable this[string name]
        {
            get { return Variables.FirstOrDefault(v => v.Name == name); }
        }

        private ILog Log = LogManager.GetLogger(typeof(Environment));

        public Environment()
        {
            Variables = new List<Variable>();
        }

        public Environment(string name, params Variable[] variables)
            : this()
        {
            Variables.AddRange(variables);
            Name = name;
        }

        public IEnumerable<Variable> EncryptVariables(string aesKey)
        {
            var variablesToEncrypt = Variables.Where(p => p.DoEncrypt).ToList();

            foreach (var variable in variablesToEncrypt)
            {
                variable.Encrypt(aesKey);
            }

            return variablesToEncrypt;
        }

        public bool DecryptVariables(string aesKey)
        {
            bool success = true;

            foreach (var variable in Variables.Where(p => p.Encrypted))
            {
                Log.DebugFormat("Decrypting variable {0}", variable.Name);
                success &= variable.Decrypt(aesKey);
            }

            return success;
        }
    }
}