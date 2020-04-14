using NJsonSchema.Annotations;

namespace k8s.CustomResources
{
    public class EmbeddedResourceAttribute : JsonSchemaExtensionDataAttribute
    {
        public EmbeddedResourceAttribute() : base("x-kubernetes-embedded-resource", true)
        {
        }
    }
}
