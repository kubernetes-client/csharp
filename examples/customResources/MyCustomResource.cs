using System.Collections.Generic;
using k8s.Models;
using Newtonsoft.Json;

namespace k8s.CustomResources
{
    [KubernetesEntity(ApiVersion = "v1", Group = "stakhov.pro", PluralName = "mycustomresources", ShortNames = new[] { "mcr" })]
    public class MyCustomResource : CustomResource, IStatus<MyCustomResourceStatus>
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
        [PrinterColumn(Name = "Description", Description = "Fancy Description")]
        [JsonProperty("description")]
        public string Description { get; set; }


        [JsonProperty("item")]
        public List<MyCustomResourceItem> Item { get; set; }

        public int Replicas { get; set; }
    }

    public class MyCustomResourceItem
    {
        [PrinterColumn(Name = "ItemName", Description = "Fancy Name")]
        public string Name { get; set; }
        [PrinterColumn(Name = "ItemValue", Description = "Fancy Value")]
        public string Value { get; set; }
    }
}
