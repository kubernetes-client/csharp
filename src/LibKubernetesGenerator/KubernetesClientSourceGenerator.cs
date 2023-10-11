using Autofac;
using Microsoft.CodeAnalysis;
using NSwag;
using Nustache.Core;
#if GENERATE_AUTOMAPPER
using System;
using System.IO;
using System.Linq;
#endif
using System.Collections.Generic;
using System.Reflection;

namespace LibKubernetesGenerator
{
    [Generator]
    public class KubernetesClientSourceGenerator : IIncrementalGenerator
    {
        private static readonly object Execlock = new object();

        private static (OpenApiDocument, IContainer) BuildContainer()
        {
            var swagger = OpenApiDocument.FromJsonAsync(EmbedResource.GetResource("swagger.json")).GetAwaiter().GetResult();
            var container = BuildContainer(swagger);
            // TODO move to Handlebars.Net
            // here is to clean up the custom helpers in static Nustache.Core.Helpers
            {
                var ch = typeof(Helpers).GetField("CustomHelpers", BindingFlags.Static | BindingFlags.NonPublic);
                ((Dictionary<string, Helper>)ch.GetValue(null)).Clear();
            }

            foreach (var helper in container.Resolve<IEnumerable<INustacheHelper>>())
            {
                helper.RegisterHelper();
            }

            return (swagger, container);
        }

        private static IContainer BuildContainer(OpenApiDocument swagger)
        {
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
            builder.RegisterType<VersionConverterStubGenerator>();
            builder.RegisterType<VersionConverterAutoMapperGenerator>();
            builder.RegisterType<VersionGenerator>();

            return builder.Build();
        }

        public void Initialize(IncrementalGeneratorInitializationContext generatorContext)
        {
#if GENERATE_BASIC
            generatorContext.RegisterPostInitializationOutput(ctx =>
            {
                lock (Execlock)
                {
                    var (swagger, container) = BuildContainer();

                    container.Resolve<VersionGenerator>().Generate(swagger, ctx);

                    container.Resolve<ModelGenerator>().Generate(swagger, ctx);
                    container.Resolve<ModelExtGenerator>().Generate(swagger, ctx);
                    container.Resolve<VersionConverterStubGenerator>().Generate(swagger, ctx);

                    container.Resolve<ApiGenerator>().Generate(swagger, ctx);
                }
            });
#endif

#if GENERATE_AUTOMAPPER
            var automappersrc = generatorContext.CompilationProvider.Select((c, _) => c.SyntaxTrees.First(s => PathSuffixMath(s.FilePath, "AutoMapper/VersionConverter.cs")));
            generatorContext.RegisterSourceOutput(automappersrc, (ctx, srctree) =>
            {
                lock (Execlock)
                {
                    var (swagger, container) = BuildContainer();

                    container.Resolve<VersionConverterAutoMapperGenerator>().Generate(swagger, ctx, srctree);
                }
            });
#endif
        }

#if GENERATE_AUTOMAPPER
        private IEnumerable<string> PathSplit(string path)
        {
            var p = path;

            while (!string.IsNullOrEmpty(p))
            {
                yield return Path.GetFileName(p);
                p = Path.GetDirectoryName(p);
            }
        }

        private bool PathSuffixMath(string path, string suffix)
        {
            var s = PathSplit(suffix).ToList();
            return PathSplit(path).Take(s.Count).SequenceEqual(s);
        }
#endif
    }
}
