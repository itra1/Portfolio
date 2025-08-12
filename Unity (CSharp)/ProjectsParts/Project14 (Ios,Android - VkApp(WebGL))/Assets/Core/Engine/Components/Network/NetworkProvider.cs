using System;

using UnityEngine.Networking;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace Core.Engine.Providers
{
	public class NetworkProvider: INetworkProvider
	{

		private async void RequestAsync(string url, string method, UnityAction<bool, string> onFinish = null, UnityAction<float> onProgress = null, CancellationTokenSource cancellationTokenSouce = default)
		{
			UnityWebRequest request;
			string data = "";

			switch (method)
			{
				case UnityWebRequest.kHttpVerbPOST:
					request = UnityWebRequest.PostWwwForm(url, data);
					break;
				case UnityWebRequest.kHttpVerbPUT:
					request = UnityWebRequest.Put(url, data);
					break;
				case UnityWebRequest.kHttpVerbDELETE:
					request = UnityWebRequest.Delete(url);
					break;
				case UnityWebRequest.kHttpVerbCREATE:
					request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbCREATE);
					break;
				case UnityWebRequest.kHttpVerbHEAD:
					request = UnityWebRequest.Head(url);
					break;
				case UnityWebRequest.kHttpVerbGET:
				default:
					request = UnityWebRequest.Get(url);
					break;
			}

			ProgressCallback progress = (onProgress == null ? null : new ProgressCallback(onProgress));
			try
			{
				await request.SendWebRequest().ToUniTask(progress, PlayerLoopTiming.TimeUpdate, cancellationTokenSouce.Token);
			}
			catch (OperationCanceledException) { }
			finally
			{
				progress = null;
				cancellationTokenSouce.Dispose();
				request.Dispose();
			}

			if (request.result == UnityWebRequest.Result.Success)
				onFinish?.Invoke(true, request.downloadHandler.text);
			else
				onFinish?.Invoke(false, request.error);

			progress = null;
			cancellationTokenSouce?.Dispose();
			request.Dispose();
		}


		private class ProgressCallback : IProgress<float>
		{
			public UnityAction<float> OnProgress { get; set; }

			public ProgressCallback(UnityAction<float> OnProgress)
			{
				this.OnProgress = OnProgress;
			}

			public void Report(float value)
			{
				OnProgress?.Invoke(value);
			}
		}

	}

}
