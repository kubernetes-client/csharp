using k8s.Models;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("cityName")]
        public string CityName { get; set; }
    }

    public class CResourceStatus : V1Status
    {
        [JsonPropertyName("temperature")]
        public string Temperature { get; set; }
    }
}
