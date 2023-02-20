using static k8s.Models.ModelVersionConverter;

namespace k8s.ModelConverter.AutoMapper;

public class AutoMapperModelVersionConverter : IModelVersionConverter
{
    public static IModelVersionConverter Instance { get; } = new AutoMapperModelVersionConverter();

    private AutoMapperModelVersionConverter()
    {
    }

    public TTo Convert<TFrom, TTo>(TFrom from)
    {
        return VersionConverter.Mapper.Map<TTo>(from);
    }
}
