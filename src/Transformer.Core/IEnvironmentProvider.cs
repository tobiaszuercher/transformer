using System.Collections.Generic;
using Transformer.Core.Model;

namespace Transformer.Core
{
    public interface IEnvironmentProvider
    {
        IEnumerable<string> GetEnvironments(bool excludeShared = true);
        Environment GetEnvironment(string environmentName);
    }
}