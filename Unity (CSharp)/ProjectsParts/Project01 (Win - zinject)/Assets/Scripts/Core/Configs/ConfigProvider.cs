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
	/// <summary>
	/// Устаревшее название - "ConfigManager"
	/// </summary>
	public class ConfigProvider : IConfig, ILateInitialized, IDisposable
	{
		private readonly IDictionary<string, string> _valuesByKey;
		private readonly CancellationTokenSource _disposeCancellationTokenSource;
		
		public bool IsInitialized { get; private set; }
		public bool IsLoaded { get; private set; }
		
		public ConfigProvider()
		{
			_valuesByKey = new Dictionary<string, string>();
			_disposeCancellationTokenSource = new CancellationTokenSource();
			
			StartDownload();
		}
		
		public bool ContainsKey(string key) => _valuesByKey.ContainsKey(key);
		public bool TryGetValue(string key, out string value) => _valuesByKey.TryGetValue(key, out value);
		public string GetValue(string key) => TryGetValue(key, out var value) ? value : null;
		
		public void Dispose()
		{
			if (!_disposeCancellationTokenSource.IsCancellationRequested)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
			}
			
			_valuesByKey.Clear();
		}
		
		private void StartDownload()
		{
#if UNITY_EDITOR
			const string fileName = "configEditor.txt";
#else
			const string fileName = "config.txt";
#endif
			
			var path = Path.Combine(Application.streamingAssetsPath, fileName);
			
			if (!Thread.CurrentThread.IsBackground && Application.isPlaying)
				DownloadAsync(path).Forget();
			else
				Download(path);
		}
		
		private async UniTaskVoid DownloadAsync(string path)
		{
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;
				
				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					
					cancellationToken.ThrowIfCancellationRequested();
					
					using (var streamReader = new StreamReader(path))
					{
						while (!streamReader.EndOfStream)
						{
							var line = await streamReader.ReadLineAsync();

							if (line == null || line.Length < 4 || line[..2] == "//")
								continue;

							var keyValuePair = line.Split('=');

							_valuesByKey.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
						}
					}
					
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
		
		private void Download(string path)
		{
			try
			{
				using var streamReader = new StreamReader(path);
				
				while (!streamReader.EndOfStream)
				{
					var line = streamReader.ReadLine();
					
					if (line == null || line.Length < 4 || line[..2] == "//")
						continue;
					
					var keyValuePair = line.Split('=');
					
					_valuesByKey.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
				}
				
				IsLoaded = true;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				IsInitialized = true;
			}
		}

		private void OnDownloadCompleted() => MessageDispatcher.SendMessage(MessageType.ConfigLoad);
	}
}