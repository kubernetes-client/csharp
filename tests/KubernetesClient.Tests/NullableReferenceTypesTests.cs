using System.Collections.Generic;
using k8s.Models;
using Xunit;

namespace k8s.Tests
{
    public class NullableReferenceTypesTests
    {
        [Fact]
        public void ContainerVolumeMountsIsNullableProperty()
        {
            // Arrange & Act
            var container = new V1Container();

            // Assert
            // VolumeMounts should be null by default (nullable property)
            Assert.Null(container.VolumeMounts);

            // This should not throw NullReferenceException anymore - users should check for null
            // container.VolumeMounts.Add(new V1VolumeMount()); // This would throw

            // Proper usage: Initialize the list first
            container.VolumeMounts = new List<V1VolumeMount>
            {
                new V1VolumeMount(),
            };

            Assert.NotNull(container.VolumeMounts);
            Assert.Single(container.VolumeMounts);
        }

        [Fact]
        public void ContainerNameIsRequiredProperty()
        {
            // Arrange & Act
            var container = new V1Container
            {
                Name = "test-container",
            };

            // Assert
            // Name is a required property (non-nullable string)
            Assert.NotNull(container.Name);
            Assert.Equal("test-container", container.Name);
        }

        [Fact]
        public void ContainerImageIsOptionalProperty()
        {
            // Arrange & Act
            var container = new V1Container();

            // Assert
            // Image is optional (nullable string)
            Assert.Null(container.Image);

            container.Image = "nginx:latest";
            Assert.Equal("nginx:latest", container.Image);
        }

        [Fact]
        public void ContainerLifecycleIsOptionalComplexProperty()
        {
            // Arrange & Act
            var container = new V1Container();

            // Assert
            // Lifecycle is optional (nullable reference type)
            Assert.Null(container.Lifecycle);

            container.Lifecycle = new V1Lifecycle();
            Assert.NotNull(container.Lifecycle);
        }

        [Fact]
        public void ContainerCollectionItemsAreNonNullable()
        {
            // Arrange
            var container = new V1Container
            {
                // Initialize the list - the list itself can be null, but items cannot be null
                VolumeMounts = new List<V1VolumeMount>
                {
                    new V1VolumeMount { Name = "vol1", MountPath = "/data", },
                    new V1VolumeMount { Name = "vol2", MountPath = "/config", },
                },
            };

            // Act & Assert
            Assert.NotNull(container.VolumeMounts);
            Assert.Equal(2, container.VolumeMounts.Count);
            Assert.All(container.VolumeMounts, vm => Assert.NotNull(vm));
        }
    }
}
