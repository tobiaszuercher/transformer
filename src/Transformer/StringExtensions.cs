using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Transformer.Cli")]

namespace Transformer
{
    public static class StringExtensions
    {
        public static void OverwriteContent(this string path, string content)
        {
            if (File.Exists(path) && File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly))
            {
                new FileInfo(path).IsReadOnly = false;
            }

            File.WriteAllText(path, content);
        } 
    }
}