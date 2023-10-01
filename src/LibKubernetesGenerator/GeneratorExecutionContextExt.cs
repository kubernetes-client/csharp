using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Nustache.Core;
using System.Text;

namespace LibKubernetesGenerator
{
    internal static class GeneratorExecutionContextExt
    {
        public static void RenderToContext(this IncrementalGeneratorPostInitializationContext context, string templatefile, object data, string generatedfile)
        {
            var template = EmbedResource.GetResource("templates."+templatefile);
            var generated = Render.StringToString(template, data);
            context.AddSource(generatedfile, SourceText.From(generated, Encoding.UTF8));
        }
    }
}
