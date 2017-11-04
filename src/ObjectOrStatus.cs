namespace k8s {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using k8s.Models;
    using Newtonsoft.Json;


    /**
     * ObjectOrStatus represents a result that is either a Kubernetes object or a Kubernetes status.
     * Some operations (e.g. Delete) routinely return an object first, and then eventually a Status.
     */
    public class ObjectOrStatus<T> {
        public ObjectOrStatus(T obj) {
            this.Object = obj;
        }
        public ObjectOrStatus(V1Status status) {
            this.Status = status;
        }

        public T Object {get;}
        public V1Status Status {get;}

        private static Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
            ContractResolver = new Microsoft.Rest.Serialization.ReadOnlyJsonContractResolver(),
            Converters = new List<JsonConverter>
            {
                new Microsoft.Rest.Serialization.Iso8601TimeSpanConverter()
            }
        };

        /**
         * Async read of an HTTP request that could return an object or a Status.
         * Catches the deserialization error and handles the alternate case.
         *
         * This is hacky, we need to rip it out once the Kubernetes OpenAPI doc supports oneOf.
         */
        public static async Task<ObjectOrStatus<T>> AsyncObjectOrStatus(Task<Microsoft.Rest.HttpOperationResponse<V1Status>> t) {
            try {
                var result = await t;
                return new ObjectOrStatus<T>(result.Body);
            } catch (Microsoft.Rest.SerializationException ex) {
                var obj = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<T>(ex.Content, settings);
                return new ObjectOrStatus<T>(obj);
            } catch (AggregateException ex) {
                foreach (var innerEx in ex.InnerExceptions) {
                    if (innerEx is Microsoft.Rest.SerializationException) {
                        var content = ((Microsoft.Rest.SerializationException)innerEx).Content;
                        var obj = Microsoft.Rest.Serialization.SafeJsonConvert.DeserializeObject<T>(content, settings);
                        return new ObjectOrStatus<T>(obj);
                    }
                }
                throw ex;
            }
        }

        /**
         * Sync read of an HTTP request that could return an object or a Status.
         * Catches the deserialization error and handles the alternate case.
         *
         * This is hacky, we need to rip it out once the Kubernetes OpenAPI doc supports oneOf.
         */
        public static ObjectOrStatus<T> ReadObjectOrStatus(Task<Microsoft.Rest.HttpOperationResponse<V1Status>> t) {
            return AsyncObjectOrStatus(t).Result;
        }
    }
}
