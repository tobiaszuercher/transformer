using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Transformer.Logging;

namespace Transformer.Model
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

        private ILog Log = LogProvider.GetCurrentClassLogger();

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
            Log.DebugFormat("Decrypt variables in environment {0}", Name);

            var variablesToEncrypt = Variables.Where(p => p.DoEncrypt).ToList();

            foreach (var variable in variablesToEncrypt)
            {
                variable.Encrypt(aesKey);
            }

            Log.DebugFormat("Encrypted {0} variable(s) in {1}", variablesToEncrypt.Count, Name);

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

        public void ChangePassword(string oldKey, string newKey)
        {
            Log.DebugFormat("Changing password for encrypted variables.");

            var variables = Variables.Where(p => p.Encrypted).ToList();

            foreach (var variable in variables)
            {
                Log.DebugFormat("Old encrypted value: {0}", variable.Value);
                if (variable.Decrypt(oldKey))
                {
                    variable.Encrypt(newKey);
                }
            }
        }
    }
}