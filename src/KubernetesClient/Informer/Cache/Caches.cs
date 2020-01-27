using System;
using System.Collections.Generic;
using k8s.Informer.Exceptions;
using k8s.Models;
using Org.BouncyCastle.Utilities;

namespace k8s.Informer.Cache
{
    public class Caches
    {
        public const string NamespaceIndex = "namespace";
        
        public static string DeletionHandlingMetaNamespaceKeyFunc<ApiType>(DeltaFifo<ApiType>.ObjectDelta obj) where ApiType : class
        {
            if (obj.IsFinalStateUnknown) 
            {
                return obj.Key;
            }
            return MetaNamespaceKeyFunc(obj);
        }
        public static string DeletionHandlingMetaNamespaceKeyFunc<ApiType>(ApiType obj) where ApiType : class => MetaNamespaceKeyFunc(obj);

        public static string MetaNamespaceKeyFunc(object obj)
        {

            V1ObjectMeta metadata;
            if (obj is string s)
            {
                return s;
            }
            else if (obj is V1ObjectMeta meta)
            {
                metadata = meta;
            }
            else
            {
                metadata = ((dynamic) obj).Metadata;
                if (metadata == null)
                {
                    throw new BadObjectException($"{obj.GetType()} does not have valid metadata");
                }
            }

            if (!string.IsNullOrEmpty(metadata.NamespaceProperty))
            {
                return $"{metadata.NamespaceProperty}/{metadata.Name}";
            }

            return metadata.Name;
        }
        
        public static List<string> MetaNamespaceIndexFunc(object obj) 
        {
                V1ObjectMeta metadata = ((dynamic) obj).Metadata;
                if (metadata == null) 
                {
                    return new List<string>();
                }
                return new List<string>() { metadata.NamespaceProperty };
        }
    }
}