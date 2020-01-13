using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace k8s
{
    public partial class Kubernetes
    {        
        public async Task<HttpOperationResponse<L>> List<T, L>(string namespaceParameter, CancellationToken cancellationToken=default(CancellationToken), bool? watch = default(bool?)) {                                    
            // Construct URL             
            // TODO: Wrap around try catch            
            var typeO = (T)Activator.CreateInstance(typeof(T));              
            var kubeApiVersion = ((String)typeO.GetType().GetProperty("KubeApiVersion").GetValue(typeO, null)).ToLower();
            var kubeKind = ((String)typeO.GetType().GetProperty("KubeKind").GetValue(typeO, null)).ToLower();              
            var kubeKindPlural = kubeKind + "s";

            var _url = new System.Uri(this.BaseUri.AbsoluteUri+"api/{apiVersion}/namespaces/{namespace}/{kind}").ToString();
            if (namespaceParameter == "") {
                _url = new System.Uri(this.BaseUri.AbsoluteUri+"api/{apiVersion}/{kind}").ToString();                
            } else {
                _url = _url.Replace("{namespace}", System.Uri.EscapeDataString(namespaceParameter));
            }
            _url = _url.Replace("{apiVersion}", System.Uri.EscapeDataString(kubeApiVersion));
            _url = _url.Replace("{kind}", System.Uri.EscapeDataString(kubeKindPlural));
            
            List<string> _queryParameters = new List<string>();
            if (watch != null)
            {
                _queryParameters.Add(string.Format("watch={0}", System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject(watch, SerializationSettings).Trim('"'))));
            }          
            
            if (_queryParameters.Count > 0)
            {
                _url += "?" + string.Join("&", _queryParameters);
            }

            HttpResponseMessage _httpResponse = null;
            string _requestContent = null;


            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();            
            _httpRequest.Method = new HttpMethod("GET");
            _httpRequest.RequestUri = new System.Uri(_url);            
            // Serialize Request           
           

            cancellationToken.ThrowIfCancellationRequested();                     
 
            _httpResponse = await HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);  

            HttpStatusCode _statusCode = _httpResponse.StatusCode;
           
            cancellationToken.ThrowIfCancellationRequested();

            string _responseContent = null;

            if ((int)_statusCode != 200 && (int)_statusCode != 401)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", _statusCode));
                if (_httpResponse.Content != null) {
                    _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else {
                    _responseContent = string.Empty;
                }
                ex.Request = new HttpRequestMessageWrapper(_httpRequest, _requestContent);
                ex.Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent);              
                _httpRequest.Dispose();
                if (_httpResponse != null)
                {
                    _httpResponse.Dispose();
                }
                throw ex;
            }

          
            // Create Result
            var _result = new HttpOperationResponse<L>();
            _result.Request = _httpRequest;
            _result.Response = _httpResponse;

            // Deserialize Response
            if ((int)_statusCode == 200)
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = SafeJsonConvert.DeserializeObject<L>(_responseContent, DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    _httpRequest.Dispose();
                    if (_httpResponse != null)
                    {
                        _httpResponse.Dispose();
                    }
                    throw new SerializationException("Unable to deserialize the response.", _responseContent, ex);
                }
            }            
            return _result; 
        }
    }
}
