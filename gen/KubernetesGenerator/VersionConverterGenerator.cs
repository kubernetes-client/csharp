using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Nustache.Core;

namespace KubernetesGenerator
{
    internal class VersionConverterGenerator
    {
        public void GenerateFromModels(string outputDirectory)
        {
            // generate version converter maps
            var allGeneratedModelClassNames = Directory
                .EnumerateFiles(Path.Combine(outputDirectory, "Models"))
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();

            var versionRegex = @"(^V|v)[0-9]+((alpha|beta)[0-9]+)?";
            var typePairs = allGeneratedModelClassNames
                .OrderBy(x => x)
                .Select(x => new
                {
                    Version = Regex.Match(x, versionRegex).Value?.ToLower(),
                    Kinda = Regex.Replace(x, versionRegex, string.Empty),
                    Type = x,
                })
                .Where(x => !string.IsNullOrEmpty(x.Version))
                .GroupBy(x => x.Kinda)
                .Where(x => x.Count() > 1)
                .SelectMany(x =>
                    x.SelectMany((value, index) => x.Skip(index + 1), (first, second) => new { first, second }))
                .OrderBy(x => x.first.Kinda)
                .ThenBy(x => x.first.Version)
                .Select(x => (ITuple)Tuple.Create(x.first.Type, x.second.Type))
                .ToList();

            var versionFile =
                File.ReadAllText(Path.Combine(outputDirectory, "..", "Versioning", "VersionConverter.cs"));
            var manualMaps = Regex.Matches(versionFile, @"\.CreateMap<(?<T1>.+?),\s?(?<T2>.+?)>")
                .Select(x => Tuple.Create(x.Groups["T1"].Value, x.Groups["T2"].Value))
                .ToList();
            var versionConverterPairs = typePairs.Except(manualMaps).ToList();

            Render.FileToFile(Path.Combine("templates", "VersionConverter.cs.template"), versionConverterPairs,
                Path.Combine(outputDirectory, "VersionConverter.cs"));
            Render.FileToFile(Path.Combine("templates", "ModelOperators.cs.template"), typePairs,
                Path.Combine(outputDirectory, "ModelOperators.cs"));
        }
    }
}
