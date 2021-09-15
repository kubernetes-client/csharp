using System.Net;
using k8s.Models;

namespace k8s.Util.Common.Generic
{
    public class KubernetesApiResponse<TDataType>
        where TDataType : IKubernetesObject
    {
        public KubernetesApiResponse(TDataType @object)
        {
            Object = @object;
            Status = null;
            HttpStatusCode = HttpStatusCode.OK; // 200
        }

        public KubernetesApiResponse(V1Status status, HttpStatusCode httpStatusCode)
        {
            Object = default(TDataType);
            Status = status;
            HttpStatusCode = httpStatusCode;
        }

        public TDataType Object { get; }

        public V1Status Status { get; }

        public HttpStatusCode HttpStatusCode { get; }

        public bool IsSuccess => ((int)HttpStatusCode > 199 && (int)HttpStatusCode < 300); // 400

        /// <summary>
        /// Throws api exception kubernetes api response on failure. This is the recommended approach to
        /// deal with errors returned from server.
        /// </summary>
        /// <returns>the kubernetes api response</returns>
        /// <exception cref="HttpListenerException">the api exception</exception>
        public KubernetesApiResponse<TDataType> ThrowsApiException()
        {
            return OnFailure(new ErrorStatusHandler());
        }

        /// <summary>
        /// Calling errorStatusHandler upon errors from server
        /// </summary>
        /// <param name="errorStatusHandler">the error status handler</param>
        /// <returns>the kubernetes api response</returns>
        public KubernetesApiResponse<TDataType> OnFailure(ErrorStatusHandler errorStatusHandler)
        {
            if (!IsSuccess && errorStatusHandler != null)
            {
                errorStatusHandler.Handle((int)HttpStatusCode, Status);
            }

            return this;
        }

        public class ErrorStatusHandler
        {
            public void Handle(int code, V1Status errorStatus)
            {
                throw new HttpListenerException(code, errorStatus?.Message);
            }
        }
    }
}
