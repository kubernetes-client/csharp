using k8s.Models;
using System.Collections.Generic;
namespace customResource
{
    public class Utils
    {
        // creats a CRD definition
        public static CustomResourceDefinition MakeCRD()
        {
            var myCRD = new CustomResourceDefinition()
            {
                Kind = "CResource",
                Group = "csharp.com",
                Version = "v1alpha1",
                PluralName = "customresources",
            };

            return myCRD;
        }

        // creats a CR instance
        public static CResource MakeCResource()
        {
            var myCResource = new CResource()
            {
                Kind = "CResource",
                ApiVersion = "csharp.com/v1alpha1",
                Metadata = new V1ObjectMeta
                {
                    Name = "cr-instance-london",
                    NamespaceProperty = "default",
                    Labels = new Dictionary<string, string>
                    {
                        {
                            "identifier", "city"
                        },
                    },
                },
                // spec
                Spec = new CResourceSpec
                {
                    CityName = "London",
                },
            };
            return myCResource;
        }
    }
}
