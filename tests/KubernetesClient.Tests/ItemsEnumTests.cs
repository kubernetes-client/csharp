using Xunit;
using k8s.Models;

namespace k8s.Tests;

public class ItemsEnumTests
{
    [Fact]
    public void EnsureIItemsEnumerable()
    {
        var pods = new V1PodList
        {
            Items = new[] { new V1Pod() },
        };

        // ensure no sytax err
        foreach (var pod in pods)
        {
            Assert.NotNull(pod);
        }
    }
}
