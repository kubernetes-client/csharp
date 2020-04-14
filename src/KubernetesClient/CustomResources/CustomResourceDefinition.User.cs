using k8s.CustomResources;

namespace k8s.Models
{
    public partial class V1CustomResourceDefinition
    {
        public static ICustomResourceDefinitionBuilder Builder => new CustomResourceDefinitionBuilder();
    }
}
