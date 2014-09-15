using Transformer.Model;

namespace Transformer
{
    public interface IEnvironmentSerializer
    {
        Environment Deserialize(string file);
    }
}