using FluentAssertions;
using k8s.Informers;
using Xunit;

namespace k8s.Tests
{
    public class KubernetesInformerOptionsBuilderTests
    {
        [Fact]
        public void SetNamespace()
        {
            var sut = KubernetesInformerOptions.Builder.NamespaceEquals("test").Build();
            sut.Namespace.Should().Be("test");
        }
        [Fact]
        public void LabelEquals()
        {
            var sut = KubernetesInformerOptions.Builder.LabelEquals("label", "test").Build();
            sut.LabelSelector.Should().Be("label=test");
        }
        [Fact]
        public void LabelContainsSingle()
        {
            var sut = KubernetesInformerOptions.Builder.LabelContains("label", "test").Build();
            sut.LabelSelector.Should().Be("label=test");
        }
        [Fact]
        public void LabelContainsMultiple()
        {
            var sut = KubernetesInformerOptions.Builder.LabelContains("label", "foo", "bar").Build();
            sut.LabelSelector.Should().Be("label in (foo,bar)");
        }
        [Fact]
        public void LabelNotEquals()
        {
            var sut = KubernetesInformerOptions.Builder.LabelEquals("label", "test").Build();
            sut.LabelSelector.Should().Be("label=test");
        }
        [Fact]
        public void LabelDoesNotContainsSingle()
        {
            var sut = KubernetesInformerOptions.Builder.LabelDoesNotContains("label", "test").Build();
            sut.LabelSelector.Should().Be("label!=test");
        }
        [Fact]
        public void LabelDoesNotContainsMultiple()
        {
            var sut = KubernetesInformerOptions.Builder.LabelDoesNotContains("label", "foo", "bar").Build();
            sut.LabelSelector.Should().Be("label notin (foo,bar)");
        }
        [Fact]
        public void HasLabel()
        {
            var sut = KubernetesInformerOptions.Builder.HasLabel("label").Build();
            sut.LabelSelector.Should().Be("label");
        }
        [Fact]
        public void DoesNotHaveLabel()
        {
            var sut = KubernetesInformerOptions.Builder.DoesNotHaveLabel("label").Build();
            sut.LabelSelector.Should().Be("!label");
        }
        [Fact]
        public void CombineMultiple_SortedAndCommandSeparated()
        {
            var sut = KubernetesInformerOptions.Builder
                .HasLabel("z")
                .LabelEquals("a", "1")
                .Build();
            sut.LabelSelector.Should().Be("a=1,z");
        }
    }
}
