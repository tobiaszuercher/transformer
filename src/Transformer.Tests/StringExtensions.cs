using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Environment = Transformer.Core.Model.Environment;

namespace Transformer.Tests
{
    public static class StringExtensions
    {
        public static string ToXmlOneLine(this string xml)
        {
            return XElement.Parse(xml).ToString(SaveOptions.DisableFormatting);
        }

        public static void SetReadOnly(this string filepath)
        {
            new FileInfo(filepath).IsReadOnly = true;
        }

        public static string MapVcsRoot(this string relativePath)
        {
            var root = new DirectoryInfo(new Uri(typeof(StringExtensions).Assembly.CodeBase).LocalPath).Parent.FullName;

            // abuse .gitignore file to find out where the root dir is
            while (!Directory.GetFiles(root).Any(f => f.Contains(".gitignore")))
            {
                Debug.WriteLine("DEBUG: " + root);
                root = Directory.GetParent(root).FullName;
            }

            return Path.Combine(root, relativePath);
        }

        public static string ToXml(this Environment target)
        {
            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(Environment));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(new StringWriter(sb), target, ns);
            
            return sb.ToString();
        }

        public static string RelativeTo(this string path, DirectoryInfo baseDir)
        {
            return Path.Combine(baseDir.FullName, path);
        }
    }
}