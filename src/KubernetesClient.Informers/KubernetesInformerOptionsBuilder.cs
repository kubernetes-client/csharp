using System;
using System.Collections.Generic;
using System.Linq;

namespace k8s.Informers
{
    public class KubernetesInformerOptionsBuilder : IKubernetesInformerOptionsBuilder
    {
        private string _namespace;
        private readonly List<string> _labelSelectors = new List<string>();

        internal KubernetesInformerOptionsBuilder()
        {
        }

        public KubernetesInformerOptionsBuilder NamespaceEquals(string value)
        {
            _namespace = value;
            return this;
        }
        public KubernetesInformerOptionsBuilder LabelEquals(string label, string value) => LabelContains(label, new[] { value });

        public KubernetesInformerOptionsBuilder LabelContains(string label, params string[] values) => LabelContains(label, (ICollection<string>)values);

        public KubernetesInformerOptionsBuilder LabelContains(string label, ICollection<string> values)
        {
            switch (values.Count)
            {
                case 0:
                    throw new InvalidOperationException($"{nameof(values)} cannot be empty");
                case 1:
                    _labelSelectors.Add($"{label}={values.First()}");
                    break;
                default:
                    _labelSelectors.Add($"{label} in ({string.Join(",", values)})");
                    break;
            }

            return this;
        }
        public KubernetesInformerOptionsBuilder LabelNotEquals(string label, string value) => LabelDoesNotContains(label, new[] { value });

        public KubernetesInformerOptionsBuilder LabelDoesNotContains(string label, params string[] values) => LabelDoesNotContains(label, (ICollection<string>)values);
        public KubernetesInformerOptionsBuilder LabelDoesNotContains(string label, ICollection<string> values)
        {
            switch (values.Count)
            {
                case 0:
                    throw new InvalidOperationException($"{nameof(values)} cannot be empty");
                case 1:
                    _labelSelectors.Add($"{label}!={values.First()}");
                    break;
                default:
                    _labelSelectors.Add($"{label} notin ({string.Join(",", values)})");
                    break;
            }

            return this;
        }
        public KubernetesInformerOptionsBuilder HasLabel(string label)
        {
            _labelSelectors.Add(label);
            return this;
        }
        public KubernetesInformerOptionsBuilder DoesNotHaveLabel(string label)
        {
            _labelSelectors.Add($"!{label}");
            return this;
        }

        public KubernetesInformerOptions Build()
        {
            string labelSelector = null;
            if (_labelSelectors.Any())
            {
                _labelSelectors.Sort();
                labelSelector = string.Join(",", _labelSelectors);
            }

            return new KubernetesInformerOptions { Namespace = _namespace, LabelSelector = labelSelector };
        }

    }
}
