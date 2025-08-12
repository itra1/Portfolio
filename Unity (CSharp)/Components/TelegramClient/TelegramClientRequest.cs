using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Telegram
{
	public partial class TelergamClient
	{
		private async UniTask<(bool, string)> Request(string url, Dictionary<string, string> formData = null)
		{
			string targetUrl = Server + url;

			Debug.Log(targetUrl);

			WWWForm form = new();

			foreach (var elem in formData)
				form.AddField(elem.Key, elem.Value);

			var request = UnityWebRequest.Post(targetUrl, form);

			request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

			var req = request.SendWebRequest();

			await UniTask.WaitUntil(() => req.isDone);

			bool result = true;
			string response = request.downloadHandler.text;

			request.Dispose();

			return (result, response);
		}
	}
}
