using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using Scriban;
using Scriban.Runtime;
using System.Text;

namespace LibKubernetesGenerator
{
    internal static class GeneratorExecutionContextExt
    {
        public static void RenderToContext(this IncrementalGeneratorPostInitializationContext context, string templatefile, ScriptObject sc, string generatedfile)
        {
            var tc = new TemplateContext();
            tc.PushGlobal(sc);
            context.RenderToContext(templatefile, tc, generatedfile);
        }

        public static void RenderToContext(this IncrementalGeneratorPostInitializationContext context, string templatefile, TemplateContext tc, string generatedfile)
        {
            var template = Template.Parse(EmbedResource.GetResource(templatefile));
            var generated = template.Render(tc);

            var syntaxTree = CSharpSyntaxTree.ParseText(generated);
            var normalized = syntaxTree.GetRoot().NormalizeWhitespace().ToFullString();
            context.AddSource(generatedfile, SourceText.From(normalized, Encoding.UTF8));
        }
    }
}
