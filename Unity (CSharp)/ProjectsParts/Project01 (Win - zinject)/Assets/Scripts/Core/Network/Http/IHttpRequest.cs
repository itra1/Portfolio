using System;
using System.Collections.Generic;
using Core.Network.Http.Data;
using UnityEngine;

namespace Core.Network.Http
{
	/// <summary>
	/// Устаревшее название - "RestManager"
	/// </summary>
	public interface IHttpRequest
	{
		void Request(string url,
			Action<string> onCompleted = null,
			Action<string> onFailure = null);
		
		void Request(string url,
			IList<(string, object)> parameters,
			Action<string> onCompleted = null,
			Action<string> onFailure = null);
		
		void Request(string url,
			string rawData,
			Action<string> onCompleted = null,
			Action<string> onFailure = null);
		
		void Request(string url,
			HttpMethodType methodType,
			Action<string> onCompleted = null, 
			Action<string> onFailure = null);
		
		void Request(string url,
			HttpMethodType methodType,
			IList<(string, object)> parameters,
			Action<string> onCompleted = null,
			Action<string> onFailure = null);
		
		void Request(string url,
			HttpMethodType methodType,
			string rawData,
			Action<string> onCompleted = null,
			Action<string> onFailure = null);
		
#if UNITY_EDITOR
		void Request(Uri uri,
			HttpMethodType methodType,
			IList<(string, object)> parameters,
			string token,
			Action<string> onCompleted = null,
			Action<string> onFailure = null);
#endif
		
		void Request(string url,
			HttpMethodType methodType,
			byte[] rawData,
			Action<string> onCompleted = null,
			Action<string> onFailure = null);
		
		void RequestBytes(string url,
			HttpMethodType methodType,
			Action<byte[]> onCompleted = null,
			Action<string> onFailure = null);
		
		void RequestTexture2D(string url,
			HttpMethodType methodType,
			bool readable,
			Action<Texture2D> onCompleted = null,
			Action<string> onFailure = null);
	}
}