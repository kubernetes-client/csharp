using System;
using NJsonSchema.Annotations;

namespace k8s.CustomResources
{
    public class PreserveUnknownFieldsAttribute : JsonSchemaExtensionDataAttribute
    {
        public PreserveUnknownFieldsAttribute() : base("x-kubernetes-preserve-unknown-fields", true)
        {
        }
    }
}
