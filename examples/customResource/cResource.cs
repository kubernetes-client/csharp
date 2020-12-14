using k8s.Models;
using System.Collections.Generic;
using k8s;
using Newtonsoft.Json;

namespace customResource
{
    public class CResource : CustomResource<CResourceSpec, CResourceStatus>
    {
        public override string ToString()
        {
            var labels = "{";
            foreach (var kvp in Metadata.Labels)
            {
                labels += kvp.Key + " : " + kvp.Value + ", ";
            }

            labels = labels.TrimEnd(',', ' ') + "}";

            return $"{Metadata.Name} (Labels: {labels}), Spec: {Spec.CityName}";
        }
    }

    public class CResourceSpec
    {
        [JsonProperty(PropertyName = "cityName")]
        public string CityName { get; set; }
    }

    public class CResourceStatus : V1Status
    {
        [JsonProperty(PropertyName = "temperature", NullValueHandling = NullValueHandling.Ignore)]
        public string Temperature { get; set; }
    }
}
