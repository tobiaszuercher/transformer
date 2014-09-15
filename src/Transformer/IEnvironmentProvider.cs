using System.Collections.Generic;

namespace PowerDeploy.Transformer
{
    public interface IEnvironmentProvider
    {
        IEnumerable<string> GetEnvironments(bool excludeShared = true);
        Environment GetEnvironment(string environmentName);
    }
}