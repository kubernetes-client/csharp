using k8s.kubectl.beta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace k8s.E2E;

public class KubectlTests
{
    [MinikubeFact]
    public void CordonTest()
    {
        var client = MinikubeTests.CreateClient();

        var node = client.CoreV1.ListNode().Items.First();
        var nodeName = node.Metadata.Name;

        var kubectl = new Kubectl(client);

        // cordon
        kubectl.Cordon(nodeName);

        // check node status
        var cordonNode = client.CoreV1.ReadNode(nodeName);
        Assert.True(cordonNode.Spec.Unschedulable);

        // uncordon
        kubectl.Uncordon(nodeName);
        cordonNode = client.CoreV1.ReadNode(nodeName);
        Assert.True(cordonNode.Spec.Unschedulable == null || cordonNode.Spec.Unschedulable == false);
    }
}
