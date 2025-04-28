using Autofac;
using Microsoft.CodeAnalysis;
using NSwag;

namespace LibKubernetesGenerator
{
    [Generator]
    public class KubernetesClientSourceGenerator : IIncrementalGenerator
    {
        private static (OpenApiDocument, IContainer) BuildContainer()
        {
            var swagger = OpenApiDocument.FromJsonAsync(EmbedResource.GetResource("swagger.json")).GetAwaiter().GetResult();
            var container = BuildContainer(swagger);
            return (swagger, container);
        }

        private static IContainer BuildContainer(OpenApiDocument swagger)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ClassNameHelper>()
                .WithParameter(new NamedParameter(nameof(swagger), swagger))
                .AsSelf()
                .AsImplementedInterfaces();

            builder.RegisterType<StringHelpers>()
                .AsImplementedInterfaces();

            builder.RegisterType<MetaHelper>()
                .AsImplementedInterfaces();

            builder.RegisterType<PluralHelper>()
                .WithParameter(new TypedParameter(typeof(OpenApiDocument), swagger))
                .AsImplementedInterfaces();

            builder.RegisterType<GeneralNameHelper>()
                .AsSelf()
                .AsImplementedInterfaces();

            builder.RegisterType<TypeHelper>()
                .AsSelf()
                .AsImplementedInterfaces();

            builder.RegisterType<ParamHelper>()
                .AsImplementedInterfaces();

            builder.RegisterType<UtilHelper>()
                .AsImplementedInterfaces();

            builder.RegisterType<ScriptObjectFactory>();

            builder.RegisterType<ModelExtGenerator>();
            builder.RegisterType<ModelGenerator>();
            builder.RegisterType<ApiGenerator>();
            builder.RegisterType<ClientSetGenerator>();
            builder.RegisterType<VersionConverterStubGenerator>();
            builder.RegisterType<VersionGenerator>();

            return builder.Build();
        }

        public void Initialize(IncrementalGeneratorInitializationContext generatorContext)
        {
#if GENERATE_BASIC
            generatorContext.RegisterPostInitializationOutput(ctx =>
            {
                var (swagger, container) = BuildContainer();

                container.Resolve<VersionGenerator>().Generate(swagger, ctx);

                container.Resolve<ModelGenerator>().Generate(swagger, ctx);
                container.Resolve<ModelExtGenerator>().Generate(swagger, ctx);
                container.Resolve<VersionConverterStubGenerator>().Generate(swagger, ctx);
                container.Resolve<ApiGenerator>().Generate(swagger, ctx);
                container.Resolve<ClientSetGenerator>().Generate(swagger, ctx);
            });
#endif

        }
    }
}
