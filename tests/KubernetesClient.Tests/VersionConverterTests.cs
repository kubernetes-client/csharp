using System.Collections.Generic;
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
        public void CanExplicitlyConvert()
        {
            var a = new V1APIService { Spec = new V1APIServiceSpec { Group = "blah" } };
            var b = (V1beta1APIService)a;
            b.Spec.Group.Should().Be("blah");
        }

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
        public void ConvertToVersion()
        {
            var src = new V1beta1APIService().Initialize();
            src.ApiVersion.Should().Be("apiregistration.k8s.io/v1beta1");
            var sut = (V1APIService)VersionConverter.ConvertToVersion(src, "v1");
            sut.Should().NotBeNull();
            sut.ApiVersion.Should().Be("apiregistration.k8s.io/v1");
        }
    }
}
