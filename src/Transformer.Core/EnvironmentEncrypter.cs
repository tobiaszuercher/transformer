using System.IO;
using System.Text.RegularExpressions;
using Transformer.Core.Logging;

namespace Transformer.Core
{
    /// <summary>
    /// Class to encrypt variables in environments. If they have a do-encrypt attribute, the value will be encrypted using the _aesKey.
    /// Before encryption:
    /// <variable name="Database.password" value="some-secret-password" do-encrypt="true" />
    /// 
    /// After encryption:
    /// <variable name="Firstname" value="(value-is-now-encrypted)" encrypted="true" />
    /// </summary>
    public class EnvironmentEncrypter
    {
        private const string RegexFormat = @"<variable (?<spaces_name>\s*)(name=""{0}"")(?<spaces_value>\s*)(value=""(?<value>[^""]+)"") do-encrypt=""([^""]*)""";

        private readonly string _aesKey;
        private IEnvironmentProvider _envProvider;
        private ILog Log = LogManager.GetLogger(typeof(EnvironmentEncrypter));

        public EnvironmentEncrypter(IEnvironmentProvider environmentProvider, string aesKey)
        {
            _envProvider = environmentProvider;
            _aesKey = aesKey;
        }

        public void EncryptAllEnvironments()
        {
            foreach (var environmentConfigFile in _envProvider.GetEnvironments(false))
            {
                EncryptEnvironmentConfig(environmentConfigFile);
            }
        }

        private Regex CreateRegexForVariable(string name)
        {
            return new Regex(string.Format(RegexFormat, name));
        }

        private void EncryptEnvironmentConfig(string configFile)
        {
            var environment = _envProvider.GetEnvironment(configFile.Replace(".xml", string.Empty)); // TODO: remove this dirty workaround
            string environmentAsText = File.ReadAllText(configFile);

            var changedVariables = environment.EncryptVariables(_aesKey);

            foreach (var variable in changedVariables)
            {
                var regex = CreateRegexForVariable(variable.Name);
                
                environmentAsText = regex.Replace(environmentAsText, @"<variable ${spaces_name}name=""" + variable.Name + @"""${spaces_value}value=""" + variable.Value + @""" encrypted=""true""");

                Log.InfoFormat("encrypting variable '{0}'", variable.Name);
            }

            File.WriteAllText(configFile, environmentAsText);
        }
    }
}