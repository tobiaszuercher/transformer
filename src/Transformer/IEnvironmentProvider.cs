using System.Collections.Generic;
using Transformer.Model;

namespace Transformer
{
    public interface IEnvironmentProvider
    {
        IEnumerable<string> GetEnvironments(bool excludeShared = true);
        Environment GetEnvironment(string environmentName);
    }
}