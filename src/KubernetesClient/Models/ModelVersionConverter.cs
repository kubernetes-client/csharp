namespace k8s.Models;

public static class ModelVersionConverter
{
    public interface IModelVersionConverter
    {
        TTo Convert<TFrom, TTo>(TFrom from);
    }

    public static IModelVersionConverter Converter { get; set; }

    internal static TTo Convert<TFrom, TTo>(TFrom from)
    {
        if (Converter == null)
        {
            throw new InvalidOperationException("Converter is not set");
        }

        return Converter.Convert<TFrom, TTo>(from);
    }
}
