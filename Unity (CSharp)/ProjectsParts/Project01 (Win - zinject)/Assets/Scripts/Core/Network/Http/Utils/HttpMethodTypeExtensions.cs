using BestHTTP;
using Core.Network.Http.Data;
using UnityEngine.Networking;

namespace Core.Network.Http.Utils
{
    public static class HttpMethodTypeExtensions
    {
        private const string Patch = "Patch";
        private const string Merge = "Merge";
        private const string Options = "Options";
        private const string Connect = "Connect";
        
        public static HTTPMethods? Convert(this HttpMethodType methodType)
        {
            return methodType switch
            {
                HttpMethodType.Get => HTTPMethods.Get,
                HttpMethodType.Head => HTTPMethods.Head,
                HttpMethodType.Put => HTTPMethods.Put,
                HttpMethodType.Post => HTTPMethods.Post,
                HttpMethodType.Delete => HTTPMethods.Delete,
                HttpMethodType.Patch => HTTPMethods.Patch,
                HttpMethodType.Merge => HTTPMethods.Merge,
                HttpMethodType.Options => HTTPMethods.Options,
                HttpMethodType.Connect => HTTPMethods.Connect,
                _ => null
            };
        }
        
        public static string Stringify(this HttpMethodType methodType)
        {
            return methodType switch
            {
                HttpMethodType.Get => UnityWebRequest.kHttpVerbGET,
                HttpMethodType.Head => UnityWebRequest.kHttpVerbHEAD,
                HttpMethodType.Post => UnityWebRequest.kHttpVerbPOST,
                HttpMethodType.Put => UnityWebRequest.kHttpVerbPUT,
                HttpMethodType.Delete => UnityWebRequest.kHttpVerbDELETE,
                HttpMethodType.Patch => Patch,
                HttpMethodType.Merge => Merge,
                HttpMethodType.Options => Options,
                HttpMethodType.Connect => Connect,
                _ => null
            };
        }
    }
}