// WARNING: DO NOT LEAVE COMMENTED CODE IN THIS FILE. IT GETS SCANNED BY GEN PROJECT SO IT CAN EXCLUDE ANY MANUALLY DEFINED MAPS

using System.Reflection;
using AutoMapper;
using k8s.Models;

namespace k8s.Versioning
{
    /// <summary>
    /// Provides mappers that converts Kubernetes models between different versions
    /// </summary>
    public static partial class VersionConverter
    {
        static VersionConverter()
        {
            UpdateMappingConfiguration(expression => { });
        }

        public static IMapper Mapper { get; private set; }
        internal static MapperConfiguration MapperConfiguration { get; private set; }

        /// <summary>
        /// Two level lookup of model types by Kind and then Version
        /// </summary>
        internal static Dictionary<string, Dictionary<string, Type>> KindVersionsMap { get; private set; }

        public static Type GetTypeForVersion<T>(string version)
        {
            return GetTypeForVersion(typeof(T), version);
        }

        public static Type GetTypeForVersion(Type type, string version)
        {
            return KindVersionsMap[type.GetKubernetesTypeMetadata().Kind][version];
        }

        public static void UpdateMappingConfiguration(Action<IMapperConfigurationExpression> configuration)
        {
            MapperConfiguration = new MapperConfiguration(cfg =>
            {
                GetConfigurations(cfg);
                configuration(cfg);
            });
            Mapper = MapperConfiguration.CreateMapper();
            KindVersionsMap = MapperConfiguration
                .GetAllTypeMaps()
                .SelectMany(x => new[] { x.Types.SourceType, x.Types.DestinationType })
                .Where(x => x.GetCustomAttribute<KubernetesEntityAttribute>() != null)
                .Select(x =>
                {
                    var attr = GetKubernetesEntityAttribute(x);
                    return new { attr.Kind, attr.ApiVersion, Type = x };
                })
                .GroupBy(x => x.Kind)
                .ToDictionary(x => x.Key, kindGroup => kindGroup
                    .GroupBy(x => x.ApiVersion)
                    .ToDictionary(
                        x => x.Key,
                        versionGroup => versionGroup.Select(x => x.Type).Distinct().Single())); // should only be one type for each Kind/Version combination
        }

        public static object ConvertToVersion(object source, string apiVersion)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var type = source.GetType();
            var attr = GetKubernetesEntityAttribute(type);
            if (attr.ApiVersion == apiVersion)
            {
                return source;
            }

            if (!KindVersionsMap.TryGetValue(attr.Kind, out var kindVersions))
            {
                throw new InvalidOperationException($"Version converter does not have any registered types for Kind `{attr.Kind}`");
            }

            if (!kindVersions.TryGetValue(apiVersion, out var targetType) || !kindVersions.TryGetValue(attr.ApiVersion, out var sourceType) || MapperConfiguration.FindTypeMapFor(sourceType, targetType) == null)
            {
                throw new InvalidOperationException($"There is no conversion mapping registered for Kind `{attr.Kind}` from ApiVersion {attr.ApiVersion} to {apiVersion}");
            }

            return Mapper.Map(source, sourceType, targetType);
        }

        private static KubernetesEntityAttribute GetKubernetesEntityAttribute(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var attr = type.GetCustomAttribute<KubernetesEntityAttribute>();
            if (attr == null)
            {
                throw new InvalidOperationException($"Type {type} does not have {nameof(KubernetesEntityAttribute)}");
            }

            return attr;
        }

        internal static void GetConfigurations(IMapperConfigurationExpression cfg)
        {
            AutoConfigurations(cfg);
            ManualConfigurations(cfg);
        }

        private static void ManualConfigurations(IMapperConfigurationExpression cfg)
        {
            cfg.AllowNullCollections = true;
            cfg.DisableConstructorMapping();
            cfg.ForAllMaps((typeMap, opt) =>
            {
                if (!typeof(IKubernetesObject).IsAssignableFrom(typeMap.Types.DestinationType))
                {
                    return;
                }

                var metadata = typeMap.Types.DestinationType.GetKubernetesTypeMetadata();
                opt.ForMember(nameof(IKubernetesObject.ApiVersion), x => x.Ignore());
                opt.ForMember(nameof(IKubernetesObject.Kind), x => x.Ignore());
                opt.AfterMap((from, to) =>
                {
                    var obj = (IKubernetesObject)to;
                    obj.ApiVersion = !string.IsNullOrEmpty(metadata.Group) ? $"{metadata.Group}/{metadata.ApiVersion}" : metadata.ApiVersion;
                    obj.Kind = metadata.Kind;
                });
            });
            cfg.CreateMap<V1Subject, V1beta1Subject>()
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceAccount, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ReverseMap();
            cfg.CreateMap<V1Subject, V1beta2Subject>()
                .ForMember(dest => dest.Group, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceAccount, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ReverseMap();

            cfg.CreateMap<V1alpha1RuntimeClass, V1RuntimeClass>()
                .ForMember(dest => dest.Handler, opt => opt.MapFrom(src => src.Spec.RuntimeHandler))
                .ForMember(dest => dest.Overhead, opt => opt.MapFrom(src => src.Spec.Overhead))
                .ForMember(dest => dest.Scheduling, opt => opt.MapFrom(src => src.Spec.Scheduling))
                .ReverseMap();
            cfg.CreateMap<V1beta1RuntimeClass, V1RuntimeClass>()
                .ForMember(dest => dest.Handler, opt => opt.MapFrom(src => src.Handler))
                .ForMember(dest => dest.Overhead, opt => opt.MapFrom(src => src.Overhead))
                .ForMember(dest => dest.Scheduling, opt => opt.MapFrom(src => src.Scheduling))
                .ReverseMap();
            cfg.CreateMap<V1alpha1RuntimeClass, V1beta1RuntimeClass>()
                .ForMember(dest => dest.Handler, opt => opt.MapFrom(src => src.Spec.RuntimeHandler))
                .ForMember(dest => dest.Overhead, opt => opt.MapFrom(src => src.Spec.Overhead))
                .ForMember(dest => dest.Scheduling, opt => opt.MapFrom(src => src.Spec.Scheduling))
                .ReverseMap();
            cfg.CreateMap<V2beta1ResourceMetricStatus, V2beta2MetricValueStatus>()
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.CurrentAverageValue))
                .ForMember(dest => dest.AverageUtilization, opt => opt.MapFrom(src => src.CurrentAverageUtilization))
                .ForMember(dest => dest.Value, opt => opt.Ignore());
            cfg.CreateMap<V2beta1ResourceMetricStatus, V2MetricValueStatus>()
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.CurrentAverageValue))
                .ForMember(dest => dest.AverageUtilization, opt => opt.MapFrom(src => src.CurrentAverageUtilization))
                .ForMember(dest => dest.Value, opt => opt.Ignore());
            cfg.CreateMap<V2beta1ResourceMetricStatus, V2beta2ResourceMetricStatus>()
                .ForMember(dest => dest.Current, opt => opt.MapFrom(src => src));
            cfg.CreateMap<V2beta2ResourceMetricStatus, V2beta1ResourceMetricStatus>()
                .ForMember(dest => dest.CurrentAverageValue, opt => opt.MapFrom(src => src.Current.AverageValue))
                .ForMember(dest => dest.CurrentAverageUtilization, opt => opt.MapFrom(src => src.Current.AverageUtilization));
            cfg.CreateMap<V2beta1ResourceMetricStatus, V2ResourceMetricStatus>()
                .ForMember(dest => dest.Current, opt => opt.MapFrom(src => src));
            cfg.CreateMap<V2ResourceMetricStatus, V2beta1ResourceMetricStatus>()
                .ForMember(dest => dest.CurrentAverageValue, opt => opt.MapFrom(src => src.Current.AverageValue))
                .ForMember(dest => dest.CurrentAverageUtilization, opt => opt.MapFrom(src => src.Current.AverageUtilization));
            cfg.CreateMap<V2beta1ResourceMetricSource, V2beta2MetricTarget>()
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.TargetAverageValue))
                .ForMember(dest => dest.Value, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.MapFrom((src, dest) => src.TargetAverageValue != null ? "AverageValue" : "Utilization"))
                .ForMember(dest => dest.AverageUtilization, opt => opt.MapFrom(src => src.TargetAverageUtilization));
            cfg.CreateMap<V2beta1ResourceMetricSource, V2MetricTarget>()
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.TargetAverageValue))
                .ForMember(dest => dest.Value, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.MapFrom((src, dest) => src.TargetAverageValue != null ? "AverageValue" : "Utilization"))
                .ForMember(dest => dest.AverageUtilization, opt => opt.MapFrom(src => src.TargetAverageUtilization));
            cfg.CreateMap<V2beta1ResourceMetricSource, V2beta2ResourceMetricSource>()
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src));
            cfg.CreateMap<V2beta2ResourceMetricSource, V2beta1ResourceMetricSource>()
                .ForMember(dest => dest.TargetAverageUtilization, opt => opt.MapFrom(src => src.Target.AverageUtilization))
                .ForMember(dest => dest.TargetAverageValue, opt => opt.MapFrom(src => src.Target.Value));
            cfg.CreateMap<V2beta1ResourceMetricSource, V2ResourceMetricSource>()
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src));
            cfg.CreateMap<V2ResourceMetricSource, V2beta1ResourceMetricSource>()
                .ForMember(dest => dest.TargetAverageUtilization, opt => opt.MapFrom(src => src.Target.AverageUtilization))
                .ForMember(dest => dest.TargetAverageValue, opt => opt.MapFrom(src => src.Target.Value));
            cfg.CreateMap<V2beta1PodsMetricStatus, V2beta2MetricValueStatus>()
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.CurrentAverageValue))
                .ForMember(dest => dest.Value, opt => opt.Ignore())
                .ForMember(dest => dest.AverageUtilization, opt => opt.Ignore());
            cfg.CreateMap<V2beta1PodsMetricStatus, V2MetricValueStatus>()
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.CurrentAverageValue))
                .ForMember(dest => dest.Value, opt => opt.Ignore())
                .ForMember(dest => dest.AverageUtilization, opt => opt.Ignore());
            cfg.CreateMap<V2beta1PodsMetricStatus, V2beta2MetricIdentifier>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MetricName))
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.Selector));
            cfg.CreateMap<V2beta1PodsMetricStatus, V2MetricIdentifier>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MetricName))
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.Selector));
            cfg.CreateMap<V2beta1PodsMetricStatus, V2beta2PodsMetricStatus>()
                .ForMember(dest => dest.Current, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Metric, opt => opt.MapFrom(src => src));
            cfg.CreateMap<V2beta2PodsMetricStatus, V2beta1PodsMetricStatus>()
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.Metric.Selector))
                .ForMember(dest => dest.CurrentAverageValue, opt => opt.MapFrom(src => src.Current.AverageValue))
                .ForMember(dest => dest.MetricName, opt => opt.MapFrom(src => src.Metric.Name));
            cfg.CreateMap<V2beta1PodsMetricStatus, V2PodsMetricStatus>()
                .ForMember(dest => dest.Current, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Metric, opt => opt.MapFrom(src => src));
            cfg.CreateMap<V2PodsMetricStatus, V2beta1PodsMetricStatus>()
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.Metric.Selector))
                .ForMember(dest => dest.CurrentAverageValue, opt => opt.MapFrom(src => src.Current.AverageValue))
                .ForMember(dest => dest.MetricName, opt => opt.MapFrom(src => src.Metric.Name));
            cfg.CreateMap<V2beta1PodsMetricSource, V2beta2MetricIdentifier>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MetricName))
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.Selector))
                .ReverseMap();
            cfg.CreateMap<V2beta1PodsMetricSource, V2MetricIdentifier>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MetricName))
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.Selector))
                .ReverseMap();
            cfg.CreateMap<V2beta1PodsMetricSource, V2beta2MetricTarget>()
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.TargetAverageValue))
                .ForMember(dest => dest.Type, opt => opt.MapFrom((src, dest) => "AverageValue"))
                .ForMember(dest => dest.Value, opt => opt.Ignore())
                .ForMember(dest => dest.AverageUtilization, opt => opt.Ignore());
            cfg.CreateMap<V2beta1PodsMetricSource, V2MetricTarget>()
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.TargetAverageValue))
                .ForMember(dest => dest.Type, opt => opt.MapFrom((src, dest) => "AverageValue"))
                .ForMember(dest => dest.Value, opt => opt.Ignore())
                .ForMember(dest => dest.AverageUtilization, opt => opt.Ignore());
            cfg.CreateMap<V2beta1PodsMetricSource, V2beta2PodsMetricSource>()
                .ForMember(dest => dest.Metric, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src));
            cfg.CreateMap<V2beta2PodsMetricSource, V2beta1PodsMetricSource>()
                .ForMember(x => x.Selector, opt => opt.MapFrom(src => src.Metric.Selector))
                .ForMember(x => x.MetricName, opt => opt.MapFrom(src => src.Metric.Name))
                .ForMember(x => x.TargetAverageValue, opt => opt.MapFrom(src => src.Target.AverageValue));
            cfg.CreateMap<V2beta1PodsMetricSource, V2PodsMetricSource>()
                .ForMember(dest => dest.Metric, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src));
            cfg.CreateMap<V2PodsMetricSource, V2beta1PodsMetricSource>()
                .ForMember(x => x.Selector, opt => opt.MapFrom(src => src.Metric.Selector))
                .ForMember(x => x.MetricName, opt => opt.MapFrom(src => src.Metric.Name))
                .ForMember(x => x.TargetAverageValue, opt => opt.MapFrom(src => src.Target.AverageValue));
            cfg.CreateMap<V2beta1ObjectMetricStatus, V2beta2MetricIdentifier>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MetricName))
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.Selector))
                .ReverseMap();
            cfg.CreateMap<V2beta1ObjectMetricStatus, V2MetricIdentifier>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MetricName))
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.Selector))
                .ReverseMap();
            cfg.CreateMap<V2beta1ObjectMetricStatus, V2beta2MetricValueStatus>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.CurrentValue))
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.AverageValue))
                .ForMember(dest => dest.AverageUtilization, opt => opt.Ignore())
                .ReverseMap();
            cfg.CreateMap<V2beta1ObjectMetricStatus, V2MetricValueStatus>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.CurrentValue))
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.AverageValue))
                .ForMember(dest => dest.AverageUtilization, opt => opt.Ignore())
                .ReverseMap();
            cfg.CreateMap<V2beta1ObjectMetricStatus, V2beta2ObjectMetricStatus>()
                .ForMember(x => x.Current, opt => opt.MapFrom(src => src))
                .ForMember(x => x.Metric, opt => opt.MapFrom(src => src))
                .ForMember(x => x.DescribedObject, opt => opt.MapFrom(src => src.Target));
            cfg.CreateMap<V2beta2ObjectMetricStatus, V2beta1ObjectMetricStatus>()
                .ForMember(x => x.CurrentValue, opt => opt.MapFrom(src => src.Current.Value))
                .ForMember(x => x.AverageValue, opt => opt.MapFrom(src => src.Current.AverageValue))
                .ForMember(x => x.MetricName, opt => opt.MapFrom(src => src.Metric.Name))
                .ForMember(x => x.Target, opt => opt.MapFrom(src => src.DescribedObject))
                .ForMember(x => x.Selector, opt => opt.MapFrom(src => src.Metric.Selector));
            cfg.CreateMap<V2beta1ObjectMetricStatus, V2ObjectMetricStatus>()
                .ForMember(x => x.Current, opt => opt.MapFrom(src => src))
                .ForMember(x => x.Metric, opt => opt.MapFrom(src => src))
                .ForMember(x => x.DescribedObject, opt => opt.MapFrom(src => src.Target));
            cfg.CreateMap<V2ObjectMetricStatus, V2beta1ObjectMetricStatus>()
                .ForMember(x => x.CurrentValue, opt => opt.MapFrom(src => src.Current.Value))
                .ForMember(x => x.AverageValue, opt => opt.MapFrom(src => src.Current.AverageValue))
                .ForMember(x => x.MetricName, opt => opt.MapFrom(src => src.Metric.Name))
                .ForMember(x => x.Target, opt => opt.MapFrom(src => src.DescribedObject))
                .ForMember(x => x.Selector, opt => opt.MapFrom(src => src.Metric.Selector));
            cfg.CreateMap<V2beta1ExternalMetricSource, V2beta2MetricTarget>()
                .ForMember(x => x.Value, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(x => x.AverageValue, opt => opt.MapFrom(src => src.TargetAverageValue))
                .ForMember(x => x.AverageUtilization, opt => opt.Ignore())
                .ForMember(x => x.Type, opt => opt.MapFrom((src, dest) => src.TargetValue != null ? "Value" : "AverageValue"));
            cfg.CreateMap<V2beta1ExternalMetricSource, V2MetricTarget>()
                .ForMember(x => x.Value, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(x => x.AverageValue, opt => opt.MapFrom(src => src.TargetAverageValue))
                .ForMember(x => x.AverageUtilization, opt => opt.Ignore())
                .ForMember(x => x.Type, opt => opt.MapFrom((src, dest) => src.TargetValue != null ? "Value" : "AverageValue"));
            cfg.CreateMap<V2beta1ExternalMetricSource, V2beta2ExternalMetricSource>()
                .ForMember(x => x.Metric, opt => opt.MapFrom(src => src))
                .ForMember(x => x.Target, opt => opt.MapFrom(src => src));
            cfg.CreateMap<V2beta2ExternalMetricSource, V2beta1ExternalMetricSource>()
                .ForMember(x => x.TargetValue, opt => opt.MapFrom(src => src.Target.Value))
                .ForMember(x => x.TargetAverageValue, opt => opt.MapFrom(src => src.Target.AverageValue))
                .ForMember(x => x.MetricName, opt => opt.MapFrom(src => src.Metric.Name))
                .ForMember(x => x.MetricSelector, opt => opt.MapFrom(src => src.Metric.Selector));
            cfg.CreateMap<V2beta1ExternalMetricSource, V2ExternalMetricSource>()
                .ForMember(x => x.Metric, opt => opt.MapFrom(src => src))
                .ForMember(x => x.Target, opt => opt.MapFrom(src => src));
            cfg.CreateMap<V2ExternalMetricSource, V2beta1ExternalMetricSource>()
                .ForMember(x => x.TargetValue, opt => opt.MapFrom(src => src.Target.Value))
                .ForMember(x => x.TargetAverageValue, opt => opt.MapFrom(src => src.Target.AverageValue))
                .ForMember(x => x.MetricName, opt => opt.MapFrom(src => src.Metric.Name))
                .ForMember(x => x.MetricSelector, opt => opt.MapFrom(src => src.Metric.Selector));
            cfg.CreateMap<V2beta1ExternalMetricStatus, V2beta2ExternalMetricStatus>()
                .ForMember(x => x.Current, opt => opt.MapFrom(src => src))
                .ForMember(x => x.Metric, opt => opt.MapFrom(src => src));
            cfg.CreateMap<V2beta2ExternalMetricStatus, V2beta1ExternalMetricStatus>()
                .ForMember(x => x.CurrentValue, opt => opt.MapFrom(src => src.Current.Value))
                .ForMember(x => x.CurrentAverageValue, opt => opt.MapFrom(src => src.Current.AverageValue))
                .ForMember(x => x.MetricName, opt => opt.MapFrom(src => src.Metric.Name))
                .ForMember(x => x.MetricSelector, opt => opt.MapFrom(src => src.Metric.Selector));
            cfg.CreateMap<V2beta1ExternalMetricStatus, V2ExternalMetricStatus>()
                .ForMember(x => x.Current, opt => opt.MapFrom(src => src))
                .ForMember(x => x.Metric, opt => opt.MapFrom(src => src));
            cfg.CreateMap<V2ExternalMetricStatus, V2beta1ExternalMetricStatus>()
                .ForMember(x => x.CurrentValue, opt => opt.MapFrom(src => src.Current.Value))
                .ForMember(x => x.CurrentAverageValue, opt => opt.MapFrom(src => src.Current.AverageValue))
                .ForMember(x => x.MetricName, opt => opt.MapFrom(src => src.Metric.Name))
                .ForMember(x => x.MetricSelector, opt => opt.MapFrom(src => src.Metric.Selector));
            cfg.CreateMap<V2beta1ExternalMetricStatus, V2beta2MetricIdentifier>()
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.MetricName))
                .ForMember(x => x.Selector, opt => opt.MapFrom(src => src.MetricSelector))
                .ReverseMap();
            cfg.CreateMap<V2beta1ExternalMetricStatus, V2MetricIdentifier>()
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.MetricName))
                .ForMember(x => x.Selector, opt => opt.MapFrom(src => src.MetricSelector))
                .ReverseMap();
            cfg.CreateMap<V2beta1ExternalMetricStatus, V2beta2MetricValueStatus>()
                .ForMember(x => x.Value, opt => opt.MapFrom(src => src.CurrentValue))
                .ForMember(x => x.AverageValue, opt => opt.MapFrom(src => src.CurrentAverageValue))
                .ForMember(x => x.AverageUtilization, opt => opt.Ignore())
                .ReverseMap();
            cfg.CreateMap<V2beta1ExternalMetricStatus, V2MetricValueStatus>()
                .ForMember(x => x.Value, opt => opt.MapFrom(src => src.CurrentValue))
                .ForMember(x => x.AverageValue, opt => opt.MapFrom(src => src.CurrentAverageValue))
                .ForMember(x => x.AverageUtilization, opt => opt.Ignore())
                .ReverseMap();
            cfg.CreateMap<V2beta1ObjectMetricSource, V2beta2MetricTarget>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(dest => dest.AverageUtilization, opt => opt.Ignore())
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.AverageValue))
                .ForMember(dest => dest.Type, opt => opt.MapFrom((src, dest) => src.TargetValue != null ? "Value" : "AverageValue"));
            cfg.CreateMap<V2beta1ObjectMetricSource, V2MetricTarget>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.TargetValue))
                .ForMember(dest => dest.AverageUtilization, opt => opt.Ignore())
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.AverageValue))
                .ForMember(dest => dest.Type, opt => opt.MapFrom((src, dest) => src.TargetValue != null ? "Value" : "AverageValue"));
            cfg.CreateMap<V2beta1ObjectMetricSource, V2beta2ObjectMetricSource>()
                .ForMember(dest => dest.Metric, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.DescribedObject, opt => opt.MapFrom(src => src.Target));
            cfg.CreateMap<V2beta2ObjectMetricSource, V2beta1ObjectMetricSource>()
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src.DescribedObject))
                .ForMember(dest => dest.MetricName, opt => opt.MapFrom(src => src.Metric.Name))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.Target.Value))
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.Target.AverageValue))
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.Metric.Selector));
            cfg.CreateMap<V2beta1ObjectMetricSource, V2ObjectMetricSource>()
                .ForMember(dest => dest.Metric, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.DescribedObject, opt => opt.MapFrom(src => src.Target));
            cfg.CreateMap<V2ObjectMetricSource, V2beta1ObjectMetricSource>()
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src.DescribedObject))
                .ForMember(dest => dest.MetricName, opt => opt.MapFrom(src => src.Metric.Name))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.Target.Value))
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.Target.AverageValue))
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.Metric.Selector));
            cfg.CreateMap<V2beta1ObjectMetricSource, V2beta2MetricIdentifier>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MetricName))
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.Selector))
                .ReverseMap();
            cfg.CreateMap<V2beta1ObjectMetricSource, V2MetricIdentifier>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MetricName))
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.Selector))
                .ReverseMap();
            cfg.CreateMap<V2beta1ExternalMetricSource, V2beta2MetricIdentifier>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MetricName))
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.MetricSelector));
            cfg.CreateMap<V2beta1ExternalMetricSource, V2MetricIdentifier>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MetricName))
                .ForMember(dest => dest.Selector, opt => opt.MapFrom(src => src.MetricSelector));
            cfg.CreateMap<V2beta2MetricTarget, V2beta1ExternalMetricSource>() // todo: not needed
                .ForMember(dest => dest.MetricName, opt => opt.Ignore())
                .ForMember(dest => dest.MetricSelector, opt => opt.Ignore())
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.TargetValue, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.TargetAverageValue, opt => opt.MapFrom(src => src.AverageValue));

            cfg.CreateMap<V1HorizontalPodAutoscalerSpec, V2beta2HorizontalPodAutoscalerSpec>()
                .ForMember(dest => dest.Metrics, opt => opt.Ignore())
                .ForMember(dest => dest.Behavior, opt => opt.Ignore())
                .ReverseMap();

            cfg.CreateMap<V1HorizontalPodAutoscalerSpec, V2HorizontalPodAutoscalerSpec>()
                .ForMember(dest => dest.Metrics, opt => opt.Ignore())
                .ForMember(dest => dest.Behavior, opt => opt.Ignore())
                .ReverseMap();

            cfg.CreateMap<V1HorizontalPodAutoscalerSpec, V2beta1HorizontalPodAutoscalerSpec>()
                .ForMember(dest => dest.Metrics, opt => opt.Ignore())
                .ReverseMap();

            cfg.CreateMap<V2beta1HorizontalPodAutoscalerSpec, V2beta2HorizontalPodAutoscalerSpec>()
                .ForMember(dest => dest.Behavior, opt => opt.Ignore())
                .ReverseMap();

            cfg.CreateMap<V2beta1HorizontalPodAutoscalerSpec, V2HorizontalPodAutoscalerSpec>()
                .ForMember(dest => dest.Behavior, opt => opt.Ignore())
                .ReverseMap();

            cfg.CreateMap<V1HorizontalPodAutoscalerStatus, V2beta1HorizontalPodAutoscalerStatus>()
                .ForMember(dest => dest.Conditions, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentMetrics, opt => opt.Ignore())
                .ReverseMap();
            cfg.CreateMap<V1HorizontalPodAutoscalerStatus, V2beta2HorizontalPodAutoscalerStatus>()
                .ForMember(dest => dest.Conditions, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentMetrics, opt => opt.Ignore())
                .ReverseMap();
            cfg.CreateMap<V1HorizontalPodAutoscalerStatus, V2HorizontalPodAutoscalerStatus>()
                .ForMember(dest => dest.Conditions, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentMetrics, opt => opt.Ignore())
                .ReverseMap();

            cfg.CreateMap<Corev1EventSeries, V1beta1EventSeries>()
                .ForMember(dest => dest.LastObservedTime, opt => opt.MapFrom(src => src.LastObservedTime))
                .ReverseMap();

            cfg.CreateMap<Corev1Event, V1beta1Event>()
                .ForMember(dest => dest.DeprecatedCount, opt => opt.Ignore())
                .ForMember(dest => dest.DeprecatedFirstTimestamp, opt => opt.MapFrom(src => src.FirstTimestamp))
                .ForMember(dest => dest.DeprecatedLastTimestamp, opt => opt.MapFrom(src => src.LastTimestamp))
                .ForMember(dest => dest.DeprecatedSource, opt => opt.MapFrom(src => src.Source))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.Regarding, opt => opt.MapFrom(src => src.InvolvedObject))
                .ForMember(dest => dest.ReportingController, opt => opt.MapFrom(src => src.ReportingComponent))
                .ReverseMap();

            cfg.CreateMap<V2beta2ContainerResourceMetricSource, V2beta1ContainerResourceMetricSource>()
                .ForMember(dest => dest.TargetAverageValue, opt => opt.MapFrom(src => src.Target.AverageValue))
                .ForMember(dest => dest.TargetAverageUtilization, opt => opt.MapFrom(src => src.Target.AverageUtilization))
                .ReverseMap();

            cfg.CreateMap<V2ContainerResourceMetricSource, V2beta1ContainerResourceMetricSource>()
                .ForMember(dest => dest.TargetAverageValue, opt => opt.MapFrom(src => src.Target.AverageValue))
                .ForMember(dest => dest.TargetAverageUtilization, opt => opt.MapFrom(src => src.Target.AverageUtilization))
                .ReverseMap();

            cfg.CreateMap<V2beta2ContainerResourceMetricStatus, V2beta1ContainerResourceMetricStatus>()
                .ForMember(dest => dest.CurrentAverageValue, opt => opt.MapFrom(src => src.Current.AverageValue))
                .ForMember(dest => dest.CurrentAverageUtilization, opt => opt.MapFrom(src => src.Current.AverageUtilization))
                .ReverseMap();

            cfg.CreateMap<V2ContainerResourceMetricStatus, V2beta1ContainerResourceMetricStatus>()
                .ForMember(dest => dest.CurrentAverageValue, opt => opt.MapFrom(src => src.Current.AverageValue))
                .ForMember(dest => dest.CurrentAverageUtilization, opt => opt.MapFrom(src => src.Current.AverageUtilization))
                .ReverseMap();

            cfg.CreateMap<V1beta1Endpoint, V1Endpoint>()
                .ForMember(dest => dest.DeprecatedTopology, opt => opt.Ignore())
                .ForMember(dest => dest.Zone, opt => opt.Ignore())
                .ReverseMap();

            cfg.CreateMap<V1beta1EndpointPort, Discoveryv1EndpointPort>()
                .ReverseMap();
        }
    }
}
