using System.Linq;
using k8s.Models;
using Microsoft.AspNetCore.JsonPatch;
using Xunit;

namespace k8s.Tests
{
    public class JsonPatchTests
    {
        [Fact]
        public void PathContainsUpperCase()
        {
            var patch = new JsonPatchDocument<V1HorizontalPodAutoscaler>();
            patch.Replace(h => h.Spec.MinReplicas, 1);

            Assert.Equal("/spec/minReplicas", patch.Operations.Single().path);
        }
    }
}
