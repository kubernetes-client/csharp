using System;
using k8s.Models;

namespace k8s.CustomResources
{
    public interface ICustomResourceDefinitionBuilderFinal
    {
        V1CustomResourceDefinition Build();
    }
}
