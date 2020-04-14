using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using k8s.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.Generation;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace k8s.CustomResources
{
    public class CustomResourceDefinitionBuilder : ICustomResourceDefinitionBuilder
    {
        private readonly List<VersionBuilder> _versions = new List<VersionBuilder>();

        private KubernetesEntityAttribute _crdMeta;

        private readonly HashSet<string> _categories = new HashSet<string>();

        private readonly HashSet<string> _shortNames = new HashSet<string>();

        private Scope _scope;
        private Version _serverVersion;

        public ICustomResourceDefinitionBuilder SetScope(Scope scope)
        {
            _scope = scope;
            return this;
        }

        public ICustomResourceDefinitionBuilder SetServerVersion(Version serverVersion)
        {
            _serverVersion = serverVersion;
            return this;
        }

        public ICustomResourceDefinitionVersionBuilder<T> AddVersion<T>()
        {

            var attr = typeof(T).GetCustomAttribute<KubernetesEntityAttribute>();
            if (attr == null)
            {
                throw new InvalidOperationException($"Custom resource must have {nameof(KubernetesEntityAttribute)} applied to it");
            }

            attr.Validate();
            if (attr.Kind == null)
            {
                attr.Kind = typeof(T).Name;
            }

            if (!_versions.Any())
            {
                _crdMeta = attr;
            }
            else
            {
                if (_crdMeta.Group != attr.Group)
                {
                    throw new InvalidOperationException($"Group is different from other versions. All versions of CRD must have the same Group");
                }

                if (_crdMeta.Kind != null && _crdMeta.Kind != attr.Kind)
                {
                    throw new InvalidOperationException($"Kind is different from other versions. All versions of CRD must have the same Kind");
                }

                if (_crdMeta.PluralName != attr.PluralName)
                {
                    throw new InvalidOperationException($"PluralName is different from other versions. All versions of CRD must have the same PluralName");
                }

                if (_crdMeta.SingularName != attr.SingularName)
                {
                    throw new InvalidOperationException($"SingularName is different from other versions. All versions of CRD must have the same SingularName");
                }

                var version = typeof(T).GetKubernetesTypeMetadata().Validate().ApiVersion;
                var conflictingVersionType = _versions.FirstOrDefault(x => x.Type.GetKubernetesTypeMetadata().ApiVersion == version);
                if (conflictingVersionType != null)
                {
                    throw new InvalidOperationException($"Version {version} is associated with type {conflictingVersionType}");
                }
            }
            if (_crdMeta.Categories != null)
            {
                foreach (var category in _crdMeta.Categories)
                {
                    _categories.Add(category);
                }
            }
            if (_crdMeta.ShortNames != null)
            {
                foreach (var shortName in _crdMeta.ShortNames)
                {
                    _shortNames.Add(shortName);
                }
            }



            var versionBuilder = new VersionBuilder<T>(this, typeof(T));
            _versions.Add(versionBuilder);
            return versionBuilder;
        }

        public V1CustomResourceDefinition Build()
        {
            var crd = new V1CustomResourceDefinition().Initialize();
            crd.Metadata.Name = $"{_crdMeta.PluralName}.{_crdMeta.Group}";
            crd.Spec = new V1CustomResourceDefinitionSpec
            {
                Group = _crdMeta.Group,
                Scope = _scope.ToString(),
                Names = new V1CustomResourceDefinitionNames
                {
                    Kind = _crdMeta.Kind,
                    Plural = _crdMeta.PluralName,
                    Singular = _crdMeta.SingularName,
                    ShortNames = _shortNames.ToList(),
                    Categories = _categories.ToList()
                },
                Versions = GetVersions()
            };
            return crd;
        }

        private List<V1CustomResourceDefinitionVersion> GetVersions()
        {
            return _versions
                .Select(versionBuilder =>
                {
                    var metadata = versionBuilder.Type.GetKubernetesTypeMetadata();
                    return new V1CustomResourceDefinitionVersion
                    {
                        Name = metadata.ApiVersion,
                        Served = versionBuilder.ShouldServe,
                        Storage = versionBuilder.ShouldStore,
                        Subresources = GetSubresources(versionBuilder),
                        Schema = new V1CustomResourceValidation(GetSchema(versionBuilder.Type)),
                        AdditionalPrinterColumns = GetColumnDefinitions(versionBuilder.Type)
                    };
                })
                .ToList();
        }


        private V1JSONSchemaProps GetSchema(Type type)
        {
            var schema = JsonSchema.FromType(type, new JsonSchemaGeneratorSettings
            {
                SchemaType = SchemaType.OpenApi3,
                FlattenInheritanceHierarchy = true,
            });
            // kubernetes does not support $ref in schemas, and has a few other quarks
            // https://github.com/kubernetes/kubernetes/issues/54579
            Inline(schema);
            schema.Definitions.Clear();

            var contractResolver = JsonSchema.CreateJsonSerializerContractResolver(NJsonSchema.SchemaType.JsonSchema);
            JsonSchemaReferenceUtilities.UpdateSchemaReferencePaths(schema, false, contractResolver);

            var serializerSettings = new JsonSerializerSettings() { Converters = new JsonConverter[] { new KubernetesStructuredSchemaConverter() } };
            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Converters = serializerSettings.Converters
            };

            var schemaJson = JsonConvert.SerializeObject(schema, Formatting.Indented, settings);
            schemaJson = JsonSchemaReferenceUtilities.ConvertPropertyReferences(schemaJson);

            var schemaToken = (JObject)JToken.Parse(schemaJson);
            RemoveFields(schemaToken, "additionalProperties");
            schemaToken.Remove("$schema");
            schemaToken.Remove("title");

            // if (_serverVersion >= new Version(1, 16, 0))
            // {
            //     MakeStructural(schemaToken);
            // }
            var schemaProps = JsonConvert.DeserializeObject<V1JSONSchemaProps>(schemaToken.ToString());
            return schemaProps;
        }



        private void Inline(JsonSchema schema)
        {
            if (schema == null)
            {
                return;
            }

            schema.IsNullableRaw = null;
            foreach (var originalProp in schema.Properties.Values.ToList())
            {
                var prop = originalProp;
                if (originalProp.Reference != null)
                {
                    schema.Properties.Remove(originalProp.Name);
                    var actualSchema = originalProp.Reference;
                    prop = new JsonSchemaProperty
                    {
                        AdditionalItemsSchema = actualSchema.AdditionalItemsSchema,
                        AdditionalPropertiesSchema = actualSchema.AdditionalPropertiesSchema,
                        AllowAdditionalItems = actualSchema.AllowAdditionalItems,
                        AllowAdditionalProperties = actualSchema.AllowAdditionalProperties,
                        Default = actualSchema.Default,
                        Description = actualSchema.Description,
                        DictionaryKey = actualSchema.DictionaryKey,
                        Discriminator = actualSchema.Discriminator,
                        DiscriminatorObject = actualSchema.DiscriminatorObject,
                        DocumentPath = actualSchema.DocumentPath,
                        EnumerationNames = actualSchema.EnumerationNames,
                        Example = actualSchema.Example,
                        ExclusiveMaximum = actualSchema.ExclusiveMaximum,
                        ExclusiveMinimum = actualSchema.ExclusiveMinimum,
                        ExtensionData = actualSchema.ExtensionData,
                        Format = actualSchema.Format,
                        Id = actualSchema.Id,
                        IsAbstract = actualSchema.IsAbstract,
                        IsDeprecated = actualSchema.IsDeprecated,
                        IsExclusiveMaximum = actualSchema.IsExclusiveMaximum,
                        IsExclusiveMinimum = actualSchema.IsExclusiveMinimum,
                        IsFlagEnumerable = actualSchema.IsFlagEnumerable,
                        IsNullableRaw = actualSchema.IsNullableRaw,
                        Item = actualSchema.Item,
                        Maximum = actualSchema.Maximum,
                        MaxItems = actualSchema.MaxItems,
                        MaxLength = actualSchema.MaxLength,
                        MaxProperties = actualSchema.MaxProperties,
                        Minimum = actualSchema.Minimum,
                        MinItems = actualSchema.MinItems,
                        MinLength = actualSchema.MinLength,
                        MinProperties = actualSchema.MinProperties,
                        MultipleOf = actualSchema.MultipleOf,
                        Not = actualSchema.Not,
                        Parent = actualSchema.Parent,
                        Pattern = actualSchema.Pattern,
                        SchemaVersion = actualSchema.SchemaVersion,
                        Title = actualSchema.Title,
                        Type = actualSchema.Type,
                        UniqueItems = actualSchema.UniqueItems,
                        Xml = actualSchema.Xml
                    };
                    foreach (var i in actualSchema.Items)
                    {
                        prop.Items.Add(i);
                    }

                    foreach (var i in actualSchema.Properties)
                    {
                        prop.Properties.Add(i);
                    }

                    foreach (var i in actualSchema.PatternProperties)
                    {
                        prop.PatternProperties.Add(i);
                    }

                    foreach (var i in actualSchema.Definitions)
                    {
                        prop.Definitions.Add(i);
                    }

                    foreach (var i in actualSchema.RequiredProperties)
                    {
                        prop.RequiredProperties.Add(i);
                    }

                    foreach (var i in actualSchema.AllOf)
                    {
                        prop.AllOf.Add(i);
                    }

                    foreach (var i in actualSchema.AnyOf)
                    {
                        prop.AnyOf.Add(i);
                    }

                    foreach (var i in actualSchema.OneOf)
                    {
                        prop.OneOf.Add(i);
                    }

                    foreach (var i in actualSchema.Enumeration)
                    {
                        prop.Enumeration.Add(i);
                    }

                    foreach (var i in actualSchema.EnumerationNames)
                    {
                        prop.EnumerationNames.Add(i);
                    }

                    prop.Parent = schema;
                    prop.IsReadOnly = originalProp.IsReadOnly;
                    prop.IsWriteOnly = originalProp.IsWriteOnly;
                    prop.GetType().GetProperty(nameof(JsonSchemaProperty.Name)).SetMethod.Invoke(prop, new[] { originalProp.Name });
                    schema.Properties.Add(prop.Name, prop);
                }

                prop.IsNullableRaw = null;

                if (prop.AdditionalItemsSchema?.Reference != null)
                {
                    prop.AdditionalItemsSchema = prop.AdditionalItemsSchema.Reference;
                }

                if (prop.AdditionalPropertiesSchema?.Reference != null)
                {
                    prop.AdditionalPropertiesSchema = prop.AdditionalPropertiesSchema.Reference;
                }

                if (prop.DictionaryKey?.Reference != null)
                {
                    prop.DictionaryKey = prop.DictionaryKey.Reference;
                }

                if (prop.Item?.Reference != null)
                {
                    prop.Item = prop.Item.Reference;
                }

                if (prop.Not?.Reference != null)
                {
                    prop.Not = prop.Not.Reference;
                }

                Inline(prop.OneOf);
                Inline(prop.AllOf);
                Inline(prop.AnyOf);

                Inline(prop.AdditionalItemsSchema);
                Inline(prop.AdditionalPropertiesSchema);
                Inline(prop.DictionaryKey);
                Inline(prop.Item);
                Inline(prop.Not);
            }
        }


        private void Inline(ICollection<JsonSchema> schemas)
        {
            var existing = schemas.ToList();
            schemas.Clear();
            foreach (var item in existing)
            {
                var actualItem = item.Reference ?? item;
                schemas.Add(actualItem);
                Inline(actualItem);
            }
        }

        private void RemoveFields(JToken token, params string[] fields)
        {
            var container = token as JContainer;
            if (container == null)
            {
                return;
            }

            var removeList = new List<JToken>();
            foreach (var el in container.Children())
            {
                if (el is JProperty p && fields.Contains(p.Name))
                {
                    removeList.Add(el);
                }
                RemoveFields(el, fields);
            }

            foreach (var el in removeList)
            {
                el.Remove();
            }
        }

        private V1CustomResourceSubresources GetSubresources(VersionBuilder versionBuilder)
        {
            if (!versionBuilder.IsScaleSubresourceEnabled && !versionBuilder.IsStatusSubresourceEnabled)
            {
                return null;
            }

            var retVal = new V1CustomResourceSubresources();
            if (versionBuilder.IsStatusSubresourceEnabled)
            {
                retVal.Status = new object();
            }

            if (versionBuilder.IsScaleSubresourceEnabled)
            {
                retVal.Scale = new V1CustomResourceSubresourceScale(versionBuilder.ScaleReplicaSpecJsonPath, versionBuilder.ScaleReplicaStatusJsonPath);
            }

            return retVal;
        }

        private List<V1CustomResourceColumnDefinition> GetColumnDefinitions(Type type)
        {
            var printerColumns = new List<V1CustomResourceColumnDefinition>();
            ProcessType(type, new HashSet<Type>(), printerColumns, new Stack<string>(), (printerAttribute, propertySchema, path) =>
                new V1CustomResourceColumnDefinition
                {
                    Description = printerAttribute.Description,
                    Name = printerAttribute.Name,
                    Priority = printerAttribute.Priority >= 0 ? printerAttribute.Priority : (int?)null,
                    Type = propertySchema.Type.ToString().ToLower(),
                    Format = propertySchema.Format,
                    JsonPath = path
                });
            return printerColumns;
        }

        private void ProcessType<T>(Type type, HashSet<Type> visitedTypes, List<T> printerColumns, Stack<string> path, Func<PrinterColumnAttribute, JsonSchema, string, T> mapper)
        {
            if (type.IsPrimitive || type == typeof(string) || visitedTypes.Contains(type))
            {
                return;
            }

            visitedTypes.Add(type);

            Type GetListType(Type typeToCheck)
            {
                return typeToCheck.GetInterfaces()
                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>))
                    .SelectMany(x => x.GetGenericArguments())
                    .FirstOrDefault();
            }
            var listType = GetListType(type);
            if (listType != null)
            {
                ProcessType(listType, visitedTypes, printerColumns, path, mapper);
                return;
            }


            foreach (var prop in type.GetProperties())
            {
                var isPropertyAList = GetListType(prop.PropertyType) != null;
                var serializablePropName = prop.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? prop.Name;
                path.Push(!isPropertyAList ? serializablePropName : $"{serializablePropName}[*]");
                var printerAttribute = prop.GetCustomAttribute<PrinterColumnAttribute>();
                if (printerAttribute != null)
                {
                    if (printerAttribute.Name == null)
                    {
                        printerAttribute.Name = prop.Name;
                    }

                    var propertySchema = JsonSchema.FromType(prop.PropertyType);
                    printerColumns.Add(mapper(printerAttribute, propertySchema, $".{string.Join(".", path.Reverse())}"));
                }
                ProcessType(prop.PropertyType, visitedTypes, printerColumns, path, mapper);
                path.Pop();
            }
        }

        public class VersionBuilder<T> : VersionBuilder, ICustomResourceDefinitionVersionBuilder<T>
        {
            public VersionBuilder(CustomResourceDefinitionBuilder parent, Type type) : base(parent, type)
            {
            }

            public ICustomResourceDefinitionVersionBuilder<T> EnableScaleSubresource<TReplicaSpec, TReplicaStatus>(
                Expression<Func<T, TReplicaSpec>> replicaSpecField,
                Expression<Func<T, TReplicaStatus>> replicaStatusField)
            {
                IsScaleSubresourceEnabled = true;
                ScaleReplicaSpecJsonPath = GetJsonPath(replicaSpecField);
                ScaleReplicaStatusJsonPath = GetJsonPath(replicaStatusField);
                return this;
            }

            private void ProcessProperty(MemberExpression s, List<string> items)
            {
                while (true)
                {
                    if (s == null)
                    {
                        return;
                    }

                    var jsonProp = s.Member.GetCustomAttribute<JsonPropertyAttribute>();
                    items.Add(jsonProp?.PropertyName ?? s.Member.Name);
                    s = s.Expression as MemberExpression;
                }
            }

            public ICustomResourceDefinitionVersionBuilder<T> IsServe(bool val = true)
            {
                ShouldServe = val;
                return this;
            }

            public ICustomResourceDefinitionVersionBuilder<T> IsStore(bool val = true)
            {
                if (val && _parent._versions.Any(x => x.ShouldStore))
                {
                    throw new InvalidOperationException("Only one version can be registered for storage");
                }

                ShouldStore = val;
                return this;
            }

            public ICustomResourceDefinitionVersionBuilder<TOther> AddVersion<TOther>() => _parent.AddVersion<TOther>();
            public V1CustomResourceDefinition Build() => _parent.Build();

            private string GetJsonPath<TResult>(Expression<Func<T, TResult>> expression)
            {
                // todo: add support for array based expressions via .Select
                var memberExpression = expression.Body as MemberExpression;
                if (memberExpression == null)
                {
                    throw new InvalidOperationException("Not supported expression type");
                }

                var items = new List<string>();
                ProcessProperty(expression.Body as MemberExpression, items);
                items.Reverse();
                return $".{string.Join(".", items)}";
            }
        }

        public class VersionBuilder
        {
            protected readonly CustomResourceDefinitionBuilder _parent;

            public VersionBuilder(CustomResourceDefinitionBuilder parent, Type type)
            {
                Type = type;
                _parent = parent;
                IsStatusSubresourceEnabled = type.GetInterfaces().Any(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IStatus<>));
            }

            internal Type Type { get; set; }
            internal bool ShouldServe { get; set; } = true;
            internal bool ShouldStore { get; set; }
            internal bool IsScaleSubresourceEnabled { get; set; }
            internal bool IsStatusSubresourceEnabled { get; set; }
            internal string ScaleReplicaSpecJsonPath { get; set; }
            internal string ScaleReplicaStatusJsonPath { get; set; }

        }
    }


}
