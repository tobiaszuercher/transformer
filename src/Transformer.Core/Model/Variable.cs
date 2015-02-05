using System.Security.Cryptography;
using System.Xml.Serialization;
using PowerDeploy.Transformer.Logging;
using Transformer.Core.Cryptography;
using Transformer.Core.Logging;

namespace Transformer.Core.Model
{
    public class Variable
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("encrypted")]
        public bool Encrypted { get; set; }

        [XmlAttribute("do-encrypt")]
        public bool DoEncrypt { get; set; }

        private ILog Log = LogManager.GetLogger(typeof(Variable));

        public Variable()
        {
        }

        public Variable(string name, string value, bool doEncrypt = false)
        {
            Name = name;
            Value = value;
            DoEncrypt = doEncrypt;

        }

        public void Encrypt(string aesKey)
        {
            Value = AES.Encrypt(Value, aesKey);
            DoEncrypt = false;
            Encrypted = true;
        }

        public bool Decrypt(string aesKey)
        {
            try
            {
                Value = AES.Decrypt(Value, aesKey);
            }
            catch (CryptographicException e)
            {
                Log.ErrorFormat("Failed to decrypt {0}, the password is wrong!", Name);
                Value = string.Format("Wrong password provided for {0}!", Name);
                
                return false;
            }

            Encrypted = false;
            
            return true;
        }
    }
}