using System.Xml.Serialization;
using PowerDeploy.Transformer.Cryptography;

namespace PowerDeploy.Transformer
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

        public void Decrypt(string aesKey)
        {
            Value = AES.Decrypt(Value, aesKey);
            Encrypted = false;
        }
    }
}