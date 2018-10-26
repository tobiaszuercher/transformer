using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Transformer.Logging;
using Transformer.Model;

namespace Transformer
{
    /// <summary>
    /// Class to encrypt variables in environments. If they have a do-encrypt attribute, the value will be encrypted using the _aesKey.
    /// Before encryption:
    /// <variable name="Database.password" value="some-secret-password" do-encrypt="true" />
    /// 
    /// After encryption:
    /// <variable name="Firstname" value="(value-is-now-encrypted)" encrypted="true" />
    /// </summary>
    /// <remarks>
    /// This seems to be a little bit nasty because we are string replacing the encrypted value with regex. We do that to keep the
    /// spacing in your xml!
    /// </remarks>
    public class EnvironmentEncrypter
    {
        private const string RegexFormat = @"<variable (?<spaces_name>\s*)(name=""{0}"")(?<spaces_value>\s*)(value=""(?<value>[^""]+)"")\s*(encrypted=""([^""]*)"")?\s*(do-encrypt=""([^""]*)"")?";
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly string _aesKey;
        private IEnvironmentProvider _envProvider;

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

        public void ChangeEncryptionKeyInAllEnvironments(string newKey)
        {
            foreach (var environmentConfigFile in _envProvider.GetEnvironments(false))
            {
                ChangeEncryptionKey(environmentConfigFile, newKey);
            }
        }

        private void ChangeEncryptionKey(string envFile, string newKey)
        {
            var environment = _envProvider.GetEnvironment(envFile.Replace(".xml", string.Empty));
            
            Log.DebugFormat("Change password from {0} to {1}", _aesKey, newKey);
            environment.ChangePassword(_aesKey, newKey);
            ReplaceVariablesInEnvironmentAsText(envFile, environment.Variables.Where(v => v.Encrypted));
        }

        private Regex CreateRegexForVariable(string name)
        {
            return new Regex(string.Format(RegexFormat, name));
        }

        private void EncryptEnvironmentConfig(string configFile)
        {
            var environment = _envProvider.GetEnvironment(configFile.Replace(".xml", string.Empty)); // TODO: remove this dirty workaround
            
            var changedVariables = environment.EncryptVariables(_aesKey);

            ReplaceVariablesInEnvironmentAsText(configFile, changedVariables);
        }

        private void ReplaceVariablesInEnvironmentAsText(string configFile, IEnumerable<Variable> changedVariables)
        {
            string environmentAsText = File.ReadAllText(configFile);
            foreach (var variable in changedVariables)
            {
                var regex = CreateRegexForVariable(variable.Name);
                environmentAsText = regex.Replace(environmentAsText, @"<variable ${spaces_name}name=""" + variable.Name + @"""${spaces_value}value=""" + variable.Value + @""" encrypted=""true""");

                Log.InfoFormat("Replace variable '{0}' value with encrypted value {1}.", variable.Name, variable.Value);
            }

            Log.DebugFormat("Writing to {0}", configFile);
            File.WriteAllText(configFile, environmentAsText);
        }
    }
}