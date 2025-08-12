using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Networks.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Game.Scripts.Providers.Networks
{
	public class NetworkProvider : INetworkProvider
	{

		public async UniTask<bool> Get(string url, UnityAction<string> callback = null)
		{
			var request = UnityWebRequest.Get(url);

			try
			{
				_ = await request.SendWebRequest();
			}
			catch { }

			var resultText = request.downloadHandler.text;

			var result = request.responseCode == ResponseCode.Success && request.result == UnityWebRequest.Result.Success;
			request.Dispose();

			callback?.Invoke(resultText);

			return result;
		}
	}
}
