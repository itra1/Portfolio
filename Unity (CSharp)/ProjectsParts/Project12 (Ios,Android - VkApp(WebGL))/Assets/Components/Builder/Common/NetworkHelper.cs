using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Builder.Common
{
	public class NetworkHelper
	{
		public IEnumerator Get(string url, UnityAction<string> OnComplete = null)
		{
			var request = UnityWebRequest.Get(url);

			_ = request.SendWebRequest();
			while (!request.isDone)
				yield return null;

			var result = request.downloadHandler.text;
			request.Dispose();

			OnComplete?.Invoke(result);
		}
		public IEnumerator Post(string url, Dictionary<string, string> data, UnityAction<string> OnComplete = null)
		{
			var request = UnityWebRequest.Post(url, data);

			Debug.Log(url);

			_ = request.SendWebRequest();
			while (!request.isDone)
				yield return null;

			var result = request.downloadHandler.text;
			request.Dispose();

			OnComplete?.Invoke(result);
		}
	}
}
