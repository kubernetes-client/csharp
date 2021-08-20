using k8s.Models;
using Xunit;
using FluentAssertions;
using k8s.Versioning;
using AutoMapper;

namespace k8s.Tests
{
    public class VersionConverterTests
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
    }
}
