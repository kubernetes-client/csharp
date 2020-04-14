using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace k8s.CustomResources
{
    /// <summary>
    /// Makes schema conform to Kubernetes structured schema spec
    /// </summary>
    /// <remarks>
    ///    See https://kubernetes.io/docs/tasks/access-kubernetes-api/custom-resources/custom-resource-definitions/#specifying-a-structural-schema
    /// </remarks>
    public class KubernetesStructuredSchemaConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var properties = (IDictionary<string, JsonSchemaProperty>)value;
            var obj = new JObject();
            foreach (var schemaProperty in properties.Values)
            {
                var serializedProperty = (JObject)JToken.FromObject(schemaProperty, serializer);
                var serializedPropertyProperties = new JObject();
                // if we have any of the "...Of" type properties, for each type listed in there, copy all the properties into a single "properties" property at same level as "...Of"
                var ofProperty = serializedProperty.Properties().FirstOrDefault(x => new[] { "oneOf", "allOf", "anyOf" }.Contains(x.Name));
                if (ofProperty != null)
                {
                    var ofValue = (JArray)ofProperty.Value;
                    foreach (var item in ofValue
                        .OfType<JObject>()
                        .Where(x => ((IDictionary<string, JToken>)x).ContainsKey("properties")))
                    {
                        void MoveToParent(string propertyName)
                        {
                            if (item.TryGetValue(propertyName, out var propValue))
                            {
                                serializedProperty[propertyName] = propValue;
                                item.Remove(propertyName);
                            }
                        }
                        // rule 3.
                        // does not set description, type, default, additionalProperties, nullable within an allOf, anyOf, oneOf or not,
                        // with the exception of the two pattern for x-kubernetes-int-or-string: true
                        MoveToParent("description");
                        MoveToParent("type"); // type should not be declared in "...Of", it should show up at parent level
                        MoveToParent("default");
                        MoveToParent("additionalProperties");
                        MoveToParent("nullable");
                        foreach (var subProp in ((JObject)item["properties"]).Properties())
                        {
                            serializedPropertyProperties.Add(subProp.Name, subProp.Value);
                        }
                    }

                    // if we have actually extracted properties from "...Of", copy em up one level
                    if (serializedPropertyProperties.HasValues)
                    {
                        serializedProperty["properties"] = serializedPropertyProperties;
                        serializedProperty["type"] = "object";
                    }

                    if (ofValue.Count == 1 && JToken.DeepEquals(ofValue.First["properties"], serializedPropertyProperties))
                    {
                        // ...Of doesn't add any value at this point cuz it's the same as the property level "properties" attribute
                        serializedProperty.Remove(ofProperty.Name);
                    }
                }

                obj.Add(schemaProperty.Name, serializedProperty);
            }

            obj.WriteTo(writer);

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            //return true;
            var result = objectType.IsGenericType && objectType.GetInterfaces().Contains(typeof(IDictionary<string, JsonSchemaProperty>));
            return result;
        }
    }
}
