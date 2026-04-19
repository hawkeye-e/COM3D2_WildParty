using System;
using System.IO;
using System.IO.Compression;

namespace COM3D2.WildParty.Plugin
{
    internal static class ResourceLoader
    {
        public static string LoadCompressedFile(string name)
        {
            using var stream = GetResourceStream(name);
            using var gzip = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzip);
            return reader.ReadToEnd();
        }

        private static Stream GetResourceStream(string name)
        {
            var assembly = typeof(ResourceLoader).Assembly;
            var fullName = $"COM3D2.WildParty.Plugin.Resources.TextResources.{name}.json.gz";
            return assembly.GetManifestResourceStream(fullName)
                   ?? throw new Exception($"Resource not found: {fullName}");
        }
    }
}

