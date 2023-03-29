using AutoMapper;
using FluentAssertions;
using k8s.ModelConverter.AutoMapper;
using k8s.Models;
using Xunit;

namespace k8s.Tests
{
    public class AutoMapperVersionConverterTests
    {
        [Fact]
        public void ConfigurationsAreValid()
        {
            var config = new MapperConfiguration(VersionConverter.GetConfigurations);
            config.AssertConfigurationIsValid();
        }

        [Theory]
        [InlineData("v1", "v1beta1", 1)]
        [InlineData("v1beta1", "v1", -1)]
        [InlineData("v1beta1", "v1alpha1", 1)]
        [InlineData("v1alpha1", "v1beta1", -1)]
        [InlineData("v1", "v1alpha1", 1)]
        [InlineData("v2alpha1", "v1", 1)]
        [InlineData("v1", "v2alpha1", -1)]
        [InlineData("v1", "v1", 0)]
        [InlineData("v2", "v2", 0)]
        [InlineData("v1beta1", "v1beta1", 0)]
        [InlineData("v1beta2", "v1beta2", 0)]
        [InlineData("v2beta2", "v2beta2", 0)]
        public void KubernetesVersionCompare(string x, string y, int expected)
        {
            KubernetesVersionComparer.Instance.Compare(x, y).Should().Be(expected);
        }

        [Fact]
        public void ObjectMapAreValid()
        {
            ModelVersionConverter.Converter = AutoMapperModelVersionConverter.Instance;
            var from = new V2HorizontalPodAutoscalerSpec(); // TODO shuold auto load all objects
            from.MaxReplicas = 234;
            var to = (V1HorizontalPodAutoscalerSpec)from;

            Assert.Equal(from.MaxReplicas, to.MaxReplicas);
        }
    }
}
