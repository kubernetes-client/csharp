using System.IO;
using System.Reflection;

namespace LibKubernetesGenerator;

internal static class EmbedResource
{
    public static string GetResource(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var resourceName = assembly.GetName().Name + "." + name;

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream ?? throw new FileNotFoundException(resourceName));
        return reader.ReadToEnd();
    }
}
