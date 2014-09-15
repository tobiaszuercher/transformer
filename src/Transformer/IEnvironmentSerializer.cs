namespace PowerDeploy.Transformer
{
    public interface IEnvironmentSerializer
    {
        Environment Deserialize(string file);
    }
}