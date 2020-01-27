using System;
using k8s.Models;
using Newtonsoft.Json;

namespace k8s.Utils
{
    public class Response<T> 
    {
        [JsonProperty("type")]
        public string Type{ get; }

        [JsonProperty("object")]
        public T Object { get; }

        public V1Status Status { get; }

        public Response(String type, T @object) 
        {
            Type = type;
            Object = @object;
        }

        public Response(String type, V1Status status) {
            Type = type;
            Status = status;
        }
    }
}