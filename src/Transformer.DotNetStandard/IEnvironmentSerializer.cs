    using Transformer.Core.Model;

namespace Transformer.Core
{
    public interface IEnvironmentSerializer
    {
        Environment Deserialize(string file);
    }
}