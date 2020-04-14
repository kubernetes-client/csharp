using System.Linq;
using FluentAssertions;
using k8s.CustomResources;
using k8s.Models;
using Newtonsoft.Json;
using Xunit;

namespace k8s.Tests
{
    public class CustomResourceDefinitionBuilderTests
    {
        [Fact]
        public void SingleVersion()
        {
            var sut = new CustomResourceDefinitionBuilder()
                .SetScope(Scope.Cluster)
                .AddVersion<MyCustomResource>()
                    .IsServe()
                    .IsStore()
                    .EnableScaleSubresource(x => x.Spec.Replicas, x => x.Status.Replicas)
                .Build();

            sut.ApiVersion.Should().Be("apiextensions.k8s.io/v1");
            sut.Kind.Should().Be("CustomResourceDefinition");
            sut.Metadata.Name.Should().Be("mycustomresources.stakhov.pro");
            sut.Spec.Group.Should().Be("stakhov.pro");
            sut.Spec.Scope.Should().Be("Cluster");
            sut.Spec.Names.ShortNames.Should().BeEquivalentTo("mcr");
            sut.Spec.Names.Categories.Should().BeEquivalentTo("mycrd");
            sut.Spec.Names.Kind.Should().Be("MyCustomResource");
            sut.Spec.Names.Singular.Should().Be("mycustomresource");
            sut.Spec.Names.Plural.Should().Be("mycustomresources");
            sut.Spec.Versions.Should().HaveCount(1);
            var version = sut.Spec.Versions.First();
            version.Name.Should().Be("v1");
            version.Served.Should().Be(true);
            version.Storage.Should().Be(true);
            version.Subresources.Scale.LabelSelectorPath.Should().BeNullOrEmpty();
            version.Subresources.Scale.SpecReplicasPath.Should().Be(".spec.replicas");
            version.Subresources.Scale.StatusReplicasPath.Should().Be(".status.Replicas");
            version.Subresources.Status.Should().NotBeNull();
            version.AdditionalPrinterColumns.Should().BeEquivalentTo(new[]
            {
                new V1CustomResourceColumnDefinition
                {
                    Name = "Name",
                    Description = "Fancy Name",
                    Format = null,
                    Type = "string",
                    Priority = 1,
                    JsonPath = ".spec.item"
                },
                new V1CustomResourceColumnDefinition
                {
                    Name = "Value",
                    Description = "Value of Item",
                    Format = "float",
                    Type = "number",
                    Priority = null,
                    JsonPath = ".spec.value"
                }
            }, opt => opt.WithoutStrictOrdering());

            version.Schema.OpenAPIV3Schema.Should().NotBeNull();
            version.Schema.OpenAPIV3Schema.Properties.Should().NotBeEmpty();
            version.Schema.OpenAPIV3Schema.Definitions.Should().BeNull();
            version.Schema.OpenAPIV3Schema.Properties.Should().ContainKeys("spec", "status");
            version.Schema.OpenAPIV3Schema.Properties.Should().NotContainKeys("apiVersion", "kind", "metadata");
        }

        [Fact]
        public void MultipleVersions()
        {
            var sut = new CustomResourceDefinitionBuilder()
                .SetScope(Scope.Cluster)
                .AddVersion<MyCustomResource>()
                    .IsServe()
                    .IsStore()
                    .EnableScaleSubresource(x => x.Spec.Replicas, x => x.Status.Replicas)
                .AddVersion<MyCustomResourceV1beta1>()
                    .IsServe(false)
                .Build();

            sut.ApiVersion.Should().Be("apiextensions.k8s.io/v1");
            sut.Kind.Should().Be("CustomResourceDefinition");
            sut.Metadata.Name.Should().Be("mycustomresources.stakhov.pro");
            sut.Spec.Group.Should().Be("stakhov.pro");
            sut.Spec.Scope.Should().Be("Cluster");
            sut.Spec.Names.ShortNames.Should().BeEquivalentTo("mcr");
            sut.Spec.Names.Categories.Should().BeEquivalentTo("mycrd");
            sut.Spec.Names.Kind.Should().Be("MyCustomResource");
            sut.Spec.Names.Singular.Should().Be("mycustomresource");
            sut.Spec.Names.Plural.Should().Be("mycustomresources");
            sut.Spec.Versions.Should().HaveCount(2);
            sut.Spec.Versions.Select(x => x.Name)
                .Should()
                .BeEquivalentTo(new[] { "v1", "v1beta1" }, opt => opt.WithoutStrictOrdering());

            var expectedPrinterColumns = new[]
            {
                new V1CustomResourceColumnDefinition
                {
                    Name = "Name",
                    Description = "Fancy Name",
                    Format = null,
                    Type = "string",
                    Priority = 1,
                    JsonPath = ".spec.item"
                },
                new V1CustomResourceColumnDefinition
                {
                    Name = "Value",
                    Description = "Value of Item",
                    Format = "float",
                    Type = "number",
                    Priority = null,
                    JsonPath = ".spec.value"
                }
            };
            var v1 = sut.Spec.Versions.First(x => x.Name == "v1");
            v1.Served.Should().Be(true);
            v1.Storage.Should().Be(true);
            v1.Subresources.Scale.LabelSelectorPath.Should().BeNullOrEmpty();
            v1.Subresources.Scale.SpecReplicasPath.Should().Be(".spec.replicas");
            v1.Subresources.Scale.StatusReplicasPath.Should().Be(".status.Replicas");
            v1.Subresources.Status.Should().NotBeNull();
            v1.AdditionalPrinterColumns.Should().BeEquivalentTo(expectedPrinterColumns, opt => opt.WithoutStrictOrdering());
            v1.Schema.OpenAPIV3Schema.Should().NotBeNull();
            v1.Schema.OpenAPIV3Schema.Properties.Should().NotBeEmpty();
            v1.Schema.OpenAPIV3Schema.Definitions.Should().BeNull();
            v1.Schema.OpenAPIV3Schema.Properties.Should().ContainKeys("spec", "status");
            v1.Schema.OpenAPIV3Schema.Properties.Should().NotContainKeys("apiVersion", "kind", "metadata");

            var v1beta1 = sut.Spec.Versions.First(x => x.Name == "v1beta1");
            v1beta1.Served.Should().Be(false);
            v1beta1.Storage.Should().Be(false);
            v1beta1.Subresources.Should().BeNull();
            v1beta1.AdditionalPrinterColumns.Should().BeEquivalentTo(expectedPrinterColumns, opt => opt.WithoutStrictOrdering());
            v1beta1.Schema.OpenAPIV3Schema.Should().NotBeNull();
            v1.Schema.OpenAPIV3Schema.Properties.Should().NotBeEmpty();
            v1.Schema.OpenAPIV3Schema.Definitions.Should().BeNull();
            v1beta1.Schema.OpenAPIV3Schema.Properties.Should().ContainKey("spec");
            v1beta1.Schema.OpenAPIV3Schema.Properties.Should().NotContainKeys("apiVersion", "kind", "metadata");
        }

        [KubernetesEntity(ApiVersion = "v1beta1", Kind = "MyCustomResource", Group = "stakhov.pro", PluralName = "mycustomresources", SingularName = "mycustomresource", Categories = new[] { "mycrd" }, ShortNames = new[] { "mcr" })]
        public class MyCustomResourceV1beta1 : CustomResource, ISpec<MyCustomResourceSpec>
        {
            [JsonProperty("spec")]
            public MyCustomResourceSpec Spec { get; set; }
        }

        [KubernetesEntity(ApiVersion = "v1", Group = "stakhov.pro", PluralName = "mycustomresources", SingularName = "mycustomresource", Categories = new[] { "mycrd" }, ShortNames = new[] { "mcr" })]
        public class MyCustomResource : CustomResource, IStatus<MyCustomResourceStatus>, ISpec<MyCustomResourceSpec>
        {
            public MyCustomResource()
            {
            }

            public MyCustomResource(string name) : base(name)
            {
            }

            [JsonProperty("spec")]
            public MyCustomResourceSpec Spec { get; set; }
            [JsonProperty("status")]
            public MyCustomResourceStatus Status { get; set; }
        }

        public class MyCustomResourceStatus
        {
            public int Replicas { get; set; }
        }

        public class MyCustomResourceSpec
        {
            [PrinterColumn(Description = "Value of Item")]
            [JsonProperty("value")]
            public float Value { get; set; }

            [PrinterColumn(Name = "Name", Description = "Fancy Name", Priority = 1)]
            [JsonProperty("item")]
            public string Item { get; set; }
            [JsonProperty("replicas")]
            public int Replicas { get; set; }
        }
    }
}
