using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using com.ootii.Messages;
using Core.Base;
using Core.Messages;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Core.Configs
{
    public class VlcConfigProvider : IVlcConfig, ILateInitialized, IDisposable
    {
	    private readonly CancellationTokenSource _disposeCancellationTokenSource;
	    
	    public bool IsInitialized { get; private set; }
	    public bool IsLoaded { get; private set; }
	    
	    public string[] Parameters { get; private set; }
	    
		public VlcConfigProvider()
		{
			_disposeCancellationTokenSource = new CancellationTokenSource();
			StartDownload();
		}
		
		public void Dispose()
		{
			if (!_disposeCancellationTokenSource.IsCancellationRequested)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}
		}

		private void StartDownload() => 
			DownloadAsync(Path.Combine(Application.streamingAssetsPath, "vlcConfig.txt")).Forget();

		private async UniTaskVoid DownloadAsync(string path)
		{
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;
				
				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					
					cancellationToken.ThrowIfCancellationRequested();
					
					var parameters = new List<string>();
					
					using (var streamReader = new StreamReader(path))
					{
						while (!streamReader.EndOfStream)
						{
							var line = await streamReader.ReadLineAsync();
							
							if (line == null || line.Length < 4 || line[..2] == "//")
								continue;
							
							parameters.Add(line);
						}
					}
					
					Parameters = parameters.ToArray();
					
					IsLoaded = true;
				}
			}
			catch (OperationCanceledException)
			{
				return;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return;
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
				
				IsInitialized = true;
			}
			
			OnDownloadCompleted();
		}

		private void OnDownloadCompleted() => MessageDispatcher.SendMessage(MessageType.VlcConfigLoad);
    }
}