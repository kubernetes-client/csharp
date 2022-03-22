using Autofac;
using Microsoft.CodeAnalysis;
using NSwag;
using Nustache.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LibKubernetesGenerator
{
    [Generator]
    public class KubernetesClientSourceGenerator : ISourceGenerator
    {
        private static object execlock = new object();

        public void ExecuteInner(GeneratorExecutionContext context)
        {
            lock (execlock)
            {
                var swaggerfile = context.AdditionalFiles.First(f => f.Path.EndsWith("swagger.json"));
                var swagger = OpenApiDocument.FromJsonAsync(swaggerfile.GetText().ToString()).GetAwaiter().GetResult();

                context.AnalyzerConfigOptions.GetOptions(swaggerfile).TryGetValue("build_metadata.AdditionalFiles.Generator", out var generatorSetting);
                var generators = new HashSet<string>(generatorSetting.Split(','));

                var builder = new ContainerBuilder();

                builder.RegisterType<ClassNameHelper>()
                    .WithParameter(new NamedParameter(nameof(swagger), swagger))
                    .AsSelf()
                    .AsImplementedInterfaces()
                    ;

                builder.RegisterType<StringHelpers>()
                    .AsImplementedInterfaces()
                    ;

                builder.RegisterType<MetaHelper>()
                    .AsImplementedInterfaces()
                    ;

                builder.RegisterType<PluralHelper>()
                    .WithParameter(new TypedParameter(typeof(OpenApiDocument), swagger))
                    .AsImplementedInterfaces()
                    ;

                builder.RegisterType<GeneralNameHelper>()
                    .AsSelf()
                    .AsImplementedInterfaces()
                    ;

                builder.RegisterType<TypeHelper>()
                    .AsSelf()
                    .AsImplementedInterfaces()
                    ;

                builder.RegisterType<ParamHelper>()
                    .AsImplementedInterfaces()
                    ;

                builder.RegisterType<UtilHelper>()
                    .AsImplementedInterfaces()
                    ;

                builder.RegisterType<ModelExtGenerator>();
                builder.RegisterType<ModelGenerator>();
                builder.RegisterType<ApiGenerator>();
                builder.RegisterType<VersionConverterGenerator>();

                var container = builder.Build();
                // TODO move to Handlebars.Net
                {
                    var ch = typeof(Helpers).GetField("CustomHelpers", BindingFlags.Static | BindingFlags.NonPublic);
                    ((Dictionary<string, Helper>)ch.GetValue(null)).Clear();
                }

                foreach (var helper in container.Resolve<IEnumerable<INustacheHelper>>())
                {
                    helper.RegisterHelper();
                }


                if (generators.Contains("api"))
                {
                    container.Resolve<ApiGenerator>().Generate(swagger, context);
                }

                if (generators.Contains("model"))
                {
                    container.Resolve<ModelGenerator>().Generate(swagger, context);
                }

                if (generators.Contains("modelext"))
                {
                    container.Resolve<ModelExtGenerator>().Generate(swagger, context);
                }

                if (generators.Contains("versionconverter"))
                {
                    container.Resolve<VersionConverterGenerator>().Generate(swagger, context);
                }
            }
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                ExecuteInner(context);
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                    "K8SCSG01",
                    e.Message,
                    e.StackTrace,
                    "Kubernetes C# code generator",
                    DiagnosticSeverity.Error,
                    true), Location.None));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
        // if (!Debugger.IsAttached)
        // {
        //     Debugger.Launch();
        // }
#endif
    }
    }
}
