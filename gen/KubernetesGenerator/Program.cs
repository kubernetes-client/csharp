using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using CommandLine;
using KubernetesGenerator;
using NSwag;

namespace KubernetesWatchGenerator
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(RunAsync).ConfigureAwait(false);
        }

        private static async Task RunAsync(Options options)
        {
            var outputDirectory = options.OutputPath;

            var swaggerCooked = await OpenApiDocument.FromFileAsync(Path.Combine(outputDirectory, "swagger.json"))
                .ConfigureAwait(false);
            var swaggerUnprocessed = await OpenApiDocument
                .FromFileAsync(Path.Combine(outputDirectory, "swagger.json.unprocessed"))
                .ConfigureAwait(false);


            var builder = new ContainerBuilder();

            builder.RegisterType<ClassNameHelper>()
                .WithParameter(new NamedParameter(nameof(swaggerCooked), swaggerCooked))
                .WithParameter(new NamedParameter(nameof(swaggerUnprocessed), swaggerUnprocessed))
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
                .WithParameter(new TypedParameter(typeof(OpenApiDocument), swaggerUnprocessed))
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
            builder.RegisterType<WatchGenerator>();
            builder.RegisterType<VersionConverterGenerator>();

            var container = builder.Build();

            foreach (var helper in container.Resolve<IEnumerable<INustacheHelper>>())
            {
                helper.RegisterHelper();
            }

            if (options.GenerateWatch)
            {
                container.Resolve<WatchGenerator>().Generate(swaggerUnprocessed, outputDirectory);
            }

            if (options.GenerateApi)
            {
                container.Resolve<ApiGenerator>().Generate(swaggerCooked, outputDirectory);
            }

            if (options.GenerateModel)
            {
                container.Resolve<ModelGenerator>().Generate(swaggerCooked, outputDirectory);
            }

            if (options.GenerateModelExt)
            {
                container.Resolve<ModelExtGenerator>().Generate(swaggerUnprocessed, outputDirectory);
            }

            if (options.GenerateVersionConverter)
            {
                container.Resolve<VersionConverterGenerator>().GenerateFromModels(outputDirectory);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1812", Justification = "Instanced in CommandLineParser")]
        public class Options
        {
            [Value(0, Required = true, HelpText = "path to src/KubernetesClient/generated")]
            public string OutputPath { get; set; }

            [Option("watch", Required = false, Default = true)]
            public bool GenerateWatch { get; set; }

            [Option("api", Required = false, Default = true)]
            public bool GenerateApi { get; set; }

            [Option("model", Required = false, Default = true)]
            public bool GenerateModel { get; set; }

            [Option("modelext", Required = false, Default = true)]
            public bool GenerateModelExt { get; set; }

            [Option("versionconverter", Required = false, Default = false)]
            public bool GenerateVersionConverter { get; set; }
        }
    }
}
