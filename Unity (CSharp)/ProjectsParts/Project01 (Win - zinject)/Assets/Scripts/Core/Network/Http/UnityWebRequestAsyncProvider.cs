using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Core.Network.Http.Data;
using Core.Network.Http.Utils;
using Core.Options;
using Cysharp.Threading.Tasks;
using Leguar.TotalJSON;
using UnityEngine;
using UnityEngine.Networking;
using Debug = Core.Logging.Debug;

namespace Core.Network.Http
{
    public class UnityWebRequestAsyncProvider : IHttpRequestAsync, IDisposable
    {
        private readonly IApplicationOptions _options;
        private readonly IHttpBaseUrl _baseUrl;
        private readonly ISet<UnityWebRequest> _requests;
        
        public UnityWebRequestAsyncProvider(IApplicationOptions options, IHttpBaseUrl baseUrl)
        {
            _options = options;
            _baseUrl = baseUrl;
            _requests = new HashSet<UnityWebRequest>();
        }
        
        public async UniTask<HttpResponseData<string>> RequestAsync(string url,
	        CancellationToken cancellationToken) => 
	        await RequestAsync(url, HttpMethodType.Get, default(string), cancellationToken);
        
        public async UniTask<HttpResponseData<string>> RequestAsync(string url,
	        IList<(string, object)> parameters,
	        CancellationToken cancellationToken) => 
	        await RequestAsync(url, HttpMethodType.Post, parameters, cancellationToken);
        
        public async UniTask<HttpResponseData<string>> RequestAsync(string url,
	        string rawData,
	        CancellationToken cancellationToken) => 
	        await RequestAsync(url, HttpMethodType.Put, rawData, cancellationToken);
        
        public async UniTask<HttpResponseData<string>> RequestAsync(string url,
	        HttpMethodType methodType,
	        CancellationToken cancellationToken) => 
	        await RequestAsync(url, methodType, default(string), cancellationToken);

        public async UniTask<HttpResponseData<string>> RequestAsync(string url,
	        HttpMethodType methodType,
	        IList<(string, object)> parameters,
	        CancellationToken cancellationToken)
        {
	        var json = new JSON();
			
	        if (parameters is { Count: > 0 })
	        {
		        foreach (var (key, value) in parameters)
			        json.Add(key, value);
	        }
			
	        return await RequestAsync(url, methodType, json.CreateString(), cancellationToken);
        }
        
        public async UniTask<HttpResponseData<string>> RequestAsync(string url,
	        HttpMethodType methodType,
	        string rawData,
	        CancellationToken cancellationToken)
        {
	        cancellationToken.ThrowIfCancellationRequested();
	        
	        var request = CreateRequest(_baseUrl.ServerApi + url, methodType);
	        
	        if (request == null)
		        throw new NullReferenceException("Request is null");
	        
	        if (_options.IsManagersLogEnabled)
		        Debug.Log($"Request: {request.uri.AbsoluteUri}" + (!string.IsNullOrEmpty(rawData) ? $". Data: {rawData}" : string.Empty));
			
	        request.SetRequestHeader("Content-Type", "application/json");
            
	        if (!string.IsNullOrEmpty(_options.ServerToken))
		        request.SetRequestHeader("Authorization", $"Bearer {_options.ServerToken}");
			
	        if (!string.IsNullOrEmpty(rawData)) 
		        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(rawData));

	        request.downloadHandler = new DownloadHandlerBuffer();
            
	        return await SendRequestAsync<string>(request, cancellationToken);
        }
        
        public async UniTask<HttpResponseData<string>> RequestAsync(string url,
	        HttpMethodType methodType,
	        byte[] bytes,
	        CancellationToken cancellationToken) 
	        => await RequestAsync<string>(url, methodType, bytes, new DownloadHandlerBuffer(), cancellationToken);
        
        public async UniTask<HttpResponseData<byte[]>> RequestBytesAsync(string url,
	        HttpMethodType type,
	        CancellationToken cancellationToken) =>
	        await RequestAsync<byte[]>(url, type, null, new DownloadHandlerBuffer(), cancellationToken);
        
        public async UniTask<HttpResponseData<Texture2D>> RequestTexture2DAsync(string url,
	        HttpMethodType type,
	        bool readable,
	        CancellationToken cancellationToken) =>
	        await RequestAsync<Texture2D>(url, type, null, new DownloadHandlerTexture(readable), cancellationToken);
        
        public void Dispose()
        {
            foreach (var request in _requests)
            {
                request.Abort();
                request.Dispose();
            }
			
            _requests.Clear();
        }
        
        private async UniTask<HttpResponseData<TResult>> RequestAsync<TResult>(string url,
	        HttpMethodType methodType,
	        byte[] rawData,
	        DownloadHandler downloadHandler,
	        CancellationToken cancellationToken)
	        where TResult : class
        {
	        cancellationToken.ThrowIfCancellationRequested();
	        
	        var request = CreateRequest(_baseUrl.ServerApi + url, methodType);
	        
	        if (request == null)
		        throw new NullReferenceException("Request is null");
	        
            if (_options.IsManagersLogEnabled)
                Debug.Log($"Request: {request.uri.AbsoluteUri}");
			
            request.SetRequestHeader("Content-Type", "application/octet-stream");
            
            if (!string.IsNullOrEmpty(_options.ServerToken))
                request.SetRequestHeader("Authorization", $"Bearer {_options.ServerToken}");
			
            if (rawData is { Length: > 0 })
	            request.uploadHandler = new UploadHandlerRaw(rawData);
            
            request.downloadHandler = downloadHandler;
            
            return await SendRequestAsync<TResult>(request, cancellationToken);
        }
        
        private UnityWebRequest CreateRequest(string url, HttpMethodType methodType)
        {
	        var method = methodType.Stringify();
	        
	        if (method == null)
		        throw new ArgumentOutOfRangeException($"Unknown HTTP method type detected: {methodType}");
	        
	        var request = new UnityWebRequest(url, method);
	        
	        request.timeout = (int) TimeSpan.FromMinutes(5.0).TotalSeconds;
	        
	        request.disposeCertificateHandlerOnDispose = true;
	        request.disposeDownloadHandlerOnDispose = true;
	        request.disposeUploadHandlerOnDispose = true;
			
	        if (_options.IsLocalSslIgnored) 
		        request.certificateHandler = new ForceAcceptAllCertificates();
	        
	        _requests.Add(request);
	        
	        return request;
        }
        
        private async UniTask<HttpResponseData<TResult>> SendRequestAsync<TResult>(UnityWebRequest request,
	        CancellationToken cancellationToken)
	        where TResult : class
        {
	        var operation = request.SendWebRequest();
	        
	        await UniTask.WaitUntil(() => operation.isDone, cancellationToken: cancellationToken);
	        
	        return HandleRequestResult<TResult>(request, cancellationToken);
        }

        private HttpResponseData<TResult> HandleRequestResult<TResult>(UnityWebRequest request,
	        CancellationToken cancellationToken)
	        where TResult : class
        {
	        _requests.Remove(request);
	        
	        try
            {
				if (cancellationToken.IsCancellationRequested)
					throw new OperationCanceledException("Request has been cancelled", cancellationToken);
				
	            var response = request.downloadHandler;
	            var responseData = new HttpResponseData<TResult>();
	            
	            switch (request.result)
	            {
		            case UnityWebRequest.Result.Success:
		            {
			            if (response.isDone)
			            {
				            var resultType = typeof(TResult);
				            
				            if (resultType == typeof(Texture2D))
				            {
					            var texture = ((DownloadHandlerTexture) response).texture;
					            
					            if (_options.IsManagersLogEnabled)
						            Debug.Log("Response: [texture2d]");
					            
					            responseData.Result = texture as TResult;
				            }
				            else if (resultType == typeof(byte[]))
				            {
					            var bytes = response.data;
					            
					            if (_options.IsManagersLogEnabled && bytes is { Length: > 0 })
						            Debug.Log("Response: [bytes]");
					            
					            responseData.Result = bytes as TResult;
				            }
				            else if (resultType == typeof(string))
				            {
					            var text = response.text;
					            
					            if (_options.IsManagersLogEnabled && !string.IsNullOrEmpty(text))
						            Debug.Log($"Response: {text}");
					            
					            responseData.Result = text as TResult;
				            }
				            else
				            {
					            Debug.LogError($"Unknown result type: \"{resultType.Name}\"");
				            }
			            }
			            else
			            {
				            if (request.error != null)
					            responseData.ErrorMessage = request.error;
				            
				            Debug.LogError($"Request is completed successfully, but the server sent an error. Message: {request.error}. Status code: {request.responseCode}. URL: {request.uri.AbsoluteUri}");
			            }

			            break;
		            }
		            case UnityWebRequest.Result.DataProcessingError:
		            {
			            responseData.ErrorMessage = request.error;
			            Debug.LogError($"Request is completed with data processing error: {request.error}. Status code: {request.responseCode}. URL: {request.uri.AbsoluteUri}");
			            break;
		            }
		            case UnityWebRequest.Result.ProtocolError:
		            {
			            responseData.ErrorMessage = request.error;
			            Debug.LogError($"Request is completed with protocol error: {request.error}. Status code: {request.responseCode}. URL: {request.uri.AbsoluteUri}");
			            break;
		            }
		            case UnityWebRequest.Result.ConnectionError:
		            {
			            responseData.ErrorMessage = $"Request is completed with connection error: {request.error}. Status code: {request.responseCode}. URL: {request.uri.AbsoluteUri}";
			            Debug.LogError(responseData.ErrorMessage);
			            break;
		            }
		            default:
		            {
			            Debug.LogError($"Unknown request result. Status code: {request.responseCode}. URL: {request.uri.AbsoluteUri}");
			            break;
		            }
	            }
	            
	            return responseData;
            }
            finally
            {
	            request.Dispose();
            }
        }
    }
}