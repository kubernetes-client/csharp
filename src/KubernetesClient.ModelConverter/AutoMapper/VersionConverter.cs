// WARNING: DO NOT LEAVE COMMENTED CODE IN THIS FILE. IT GETS SCANNED BY GEN PROJECT SO IT CAN EXCLUDE ANY MANUALLY DEFINED MAPS

using AutoMapper;
#if NET6_0_OR_GREATER
using AutoMapper.Internal;
#endif
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace k8s.ModelConverter.AutoMapper;

/// <summary>
/// Provides mappers that converts Kubernetes models between different versions
/// </summary>
internal static partial class VersionConverter
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
        Mapper = MapperConfiguration
#if NET6_0_OR_GREATER
            .Internal()
#endif
            .CreateMapper();
        KindVersionsMap = MapperConfiguration
#if NET6_0_OR_GREATER
            .Internal()
#endif
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

        if (!kindVersions.TryGetValue(apiVersion, out var targetType) || !kindVersions.TryGetValue(attr.ApiVersion, out var sourceType) ||
            MapperConfiguration
#if NET6_0_OR_GREATER
            .Internal()
#endif
            .FindTypeMapFor(sourceType, targetType) == null)
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
        cfg
#if NET6_0_OR_GREATER
            .Internal()
#endif
            .ForAllMaps((typeMap, opt) =>
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

        cfg.CreateMap<V1HorizontalPodAutoscalerSpec, V2HorizontalPodAutoscalerSpec>()
            .ForMember(dest => dest.Metrics, opt => opt.Ignore())
            .ForMember(dest => dest.Behavior, opt => opt.Ignore())
            .ReverseMap();


        cfg.CreateMap<V1HorizontalPodAutoscalerStatus, V2HorizontalPodAutoscalerStatus>()
            .ForMember(dest => dest.Conditions, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentMetrics, opt => opt.Ignore())
            .ReverseMap();

        cfg.CreateMap<V1alpha2ResourceClaim, V1ResourceClaim>()
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ReverseMap();

        cfg.CreateMap<V1beta3PolicyRulesWithSubjects, V1PolicyRulesWithSubjects>()
            .ForMember(dest => dest.Subjects, opt => opt.Ignore())
            .ReverseMap();
    }
}
