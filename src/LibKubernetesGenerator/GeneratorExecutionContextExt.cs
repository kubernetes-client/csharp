using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Nustache.Core;
using System.IO;
using System.Text;

namespace LibKubernetesGenerator
{
    internal static class GeneratorExecutionContextExt
    {
        public static void RenderToContext(this GeneratorExecutionContext context, string templatefile, object data, string generatedfile)
        {
            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out var root);
            var generated = Render.FileToString(Path.Combine(root, "..", "LibKubernetesGenerator", "templates", templatefile), data);
            context.AddSource(generatedfile, SourceText.From(generated, Encoding.UTF8));
        }
    }
}
