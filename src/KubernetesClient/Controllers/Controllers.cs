using System;
using k8s.Controllers.Reconciler;
using k8s.Models;
using Microsoft.CSharp.RuntimeBinder;

namespace k8s.Controllers
{
    public class Controllers
    {
        public static Func<TApiType, Request> DefaultReflectiveKeyFunc<TApiType>() 
        {
            return obj => 
            {
                try 
                {
                    V1ObjectMeta objectMeta = ((dynamic)obj).Metadata;
                    return new Request(objectMeta.NamespaceProperty, objectMeta.Name);
                }
                catch (RuntimeBinderException) 
                {
                    //todo: figure out what is the best way to get logger into static class
                    Console.WriteLine($"Fail to access object-meta from {obj.GetType()}");
                    // _log.error("Fail to access object-meta from {}..", obj.getClass());
                    return null;
                }
            };
        }
    }
}