namespace Transformer.Core
{
    public interface IEnvironmentFolderLocator
    {
        string EnvironmentFolder { get; }
        string GetEnvironmentFile(string environment);
    }
}