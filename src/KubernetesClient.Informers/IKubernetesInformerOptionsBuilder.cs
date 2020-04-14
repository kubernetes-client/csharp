using System.Collections.Generic;

namespace k8s.Informers
{
    public interface IKubernetesInformerOptionsBuilder
    {
        KubernetesInformerOptionsBuilder NamespaceEquals(string value);
        KubernetesInformerOptionsBuilder LabelEquals(string label, string value);
        KubernetesInformerOptionsBuilder LabelNotEquals(string label, string value);
        KubernetesInformerOptionsBuilder LabelContains(string label, params string[] values);
        KubernetesInformerOptionsBuilder LabelContains(string label, ICollection<string> values);
        KubernetesInformerOptionsBuilder LabelDoesNotContains(string label, params string[] values);
        KubernetesInformerOptionsBuilder LabelDoesNotContains(string label, ICollection<string> values);
        KubernetesInformerOptionsBuilder HasLabel(string label);
        KubernetesInformerOptionsBuilder DoesNotHaveLabel(string label);
        KubernetesInformerOptions Build();
    }
}
