using System;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Core.FileResources;
using Core.FileResources.Customizing.Category;
using Core.FileResources.Info;
using Core.Materials.Data;
using Core.Materials.Storage;
using Core.Messages;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Options;
using Core.UI.Audio.Enums;
using Core.Utils;
using Cysharp.Threading.Tasks;
using UI.Audio;
using UI.Canvas.Presenter;
using UI.MusicPlayer.Controller.Converter;
using UI.MusicPlayer.Controller.Converter.Data;
using UI.MusicPlayer.Presenter;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace UI.MusicPlayer.Controller
{
	public class MusicPlayerController : IMusicPlayerController, IDisposable
	{
		private readonly IApplicationOptions _options;
		private readonly IMaterialDataStorage _materials;
		private readonly IResourceProvider _resources;
		private readonly IAudioMixerGroupProvider _provider;
		private readonly IAudioClipInfoResolver _resolver;
		private readonly IOutgoingStateController _outgoingState;
		
		private IMusicPlayerPresenter _presenter;
		
		private CancellationTokenSource _disposeCancellationTokenSource;
		private CancellationTokenSource _updateCancellationTokenSource;
		
		private CancellationTokenSourceProxy _downloadFileCancellationTokenSource;
		
		private bool _isFirstSelection;
		
		public MusicPlayerController(IApplicationOptions options,
			IMaterialDataStorage materials,
			IResourceProvider resources,
			IAudioMixerGroupProvider provider,
			ICanvasPresenter root,
			IAudioClipInfoResolver resolver,
			IOutgoingStateController outgoingState)
		{
			_options = options;
			_materials = materials;
			_resources = resources;
			_provider = provider;
			_resolver = resolver;
			_outgoingState = outgoingState;
			_presenter = root.MusicPlayer;
			_disposeCancellationTokenSource = new CancellationTokenSource();
			
			_presenter.Initialize();
			
			MessageDispatcher.AddListener(MessageType.AppInitialize, OnApplicationInitialized);
			MessageDispatcher.AddListener(MessageType.AutoMaterialDataPreloadComplete, OnAutoMaterialDataPreloadCompleted);
			MessageDispatcher.AddListener(MessageType.PlaybackPause, OnPlaybackPaused);
		}
		
		public void Dispose()
		{
			MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
			MessageDispatcher.RemoveListener(MessageType.AutoMaterialDataPreloadComplete, OnAutoMaterialDataPreloadCompleted);
			MessageDispatcher.RemoveListener(MessageType.PlaybackPause, OnPlaybackPaused);
			
			MessageDispatcher.RemoveListener(MessageType.MusicPlayerLoopToggle, OnMusicPlayerLoopToggle);
			MessageDispatcher.RemoveListener(MessageType.MusicPlayerPlayToggle, OnMusicPlayerPlayToggle);
			MessageDispatcher.RemoveListener(MessageType.MusicPlayerTrackNext, OnMusicPlayerTrackNext);
			MessageDispatcher.RemoveListener(MessageType.MusicPlayerTrackPrevious, OnMusicPlayerTrackPrevious);
			MessageDispatcher.RemoveListener(MessageType.MusicPlayerTrackSet, OnMusicPlayerTrackSet);
			MessageDispatcher.RemoveListener(MessageType.MusicPlayerTrackReset, OnMusicPlayerTrackReset);
			MessageDispatcher.RemoveListener(MessageType.MusicPlayerTimeSet, OnMusicPlayerTimeSet);
			MessageDispatcher.RemoveListener(MessageType.AudioPreload, OnAudioLoaded);
			MessageDispatcher.RemoveListener(MessageType.AudioUnload, OnAudioUnloaded);
			MessageDispatcher.RemoveListener(MessageType.AudioUpdate, OnAudioUpdated);
			
			if (_downloadFileCancellationTokenSource != null)
			{
				if (!_downloadFileCancellationTokenSource.IsDisposed && !_downloadFileCancellationTokenSource.IsCancellationRequested)
					_downloadFileCancellationTokenSource.Cancel();
		        
				_downloadFileCancellationTokenSource = null;
			}
			
			if (_disposeCancellationTokenSource != null)
			{
				_disposeCancellationTokenSource.Cancel();
				_disposeCancellationTokenSource.Dispose();
				_disposeCancellationTokenSource = null;
			}
			
			if (_updateCancellationTokenSource != null)
			{
				_updateCancellationTokenSource.Cancel();
				_updateCancellationTokenSource.Dispose();
				_updateCancellationTokenSource = null;
			}
			
			if (_presenter != null)
			{
				_presenter.PlaybackCompleted -= OnPlaybackCompleted;
				_presenter.Unload();
				_presenter = null;
			}

			_isFirstSelection = false;
		}

		private async UniTaskVoid StartupAsync()
		{
			if (await PreloadAsync())
			{
				_presenter.PlaybackCompleted += OnPlaybackCompleted;
				
				MessageDispatcher.AddListener(MessageType.MusicPlayerLoopToggle, OnMusicPlayerLoopToggle);
				MessageDispatcher.AddListener(MessageType.MusicPlayerPlayToggle, OnMusicPlayerPlayToggle);
				MessageDispatcher.AddListener(MessageType.MusicPlayerTrackNext, OnMusicPlayerTrackNext);
				MessageDispatcher.AddListener(MessageType.MusicPlayerTrackPrevious, OnMusicPlayerTrackPrevious);
				MessageDispatcher.AddListener(MessageType.MusicPlayerTrackSet, OnMusicPlayerTrackSet);
				MessageDispatcher.AddListener(MessageType.MusicPlayerTrackReset, OnMusicPlayerTrackReset);
				MessageDispatcher.AddListener(MessageType.MusicPlayerTimeSet, OnMusicPlayerTimeSet);
				MessageDispatcher.AddListener(MessageType.AudioPreload, OnAudioLoaded);
				MessageDispatcher.AddListener(MessageType.AudioUnload, OnAudioUnloaded);
				MessageDispatcher.AddListener(MessageType.AudioUpdate, OnAudioUpdated);
				
				UpdateAsync().Forget();
			}
			else
			{
				Debug.LogError("The music player was unable to successfully preload audio materials and then add tracks");
			}
			
			MessageDispatcher.SendMessage(MessageType.MusicPlayerPreloadCompleted, EnumMessageDelay.IMMEDIATE);
		}
		
		private async UniTask<bool> PreloadAsync()
		{
			try
			{
				var cancellationToken = _disposeCancellationTokenSource.Token;
				
				var materials = _materials.GetList<AudioMaterialData>();
				
				var audioTrackInfoList = new List<AudioTrackInfo>();
				var audioClipInfoList = new List<AudioClipInfo>();
				
				foreach (var material in materials)
				{
					if (material == null)
						continue;
					
					var bytes = await ConfirmMaterialAsync(material, cancellationToken);
					
					if (bytes != null)
						audioTrackInfoList.Add(new AudioTrackInfo(material, bytes));
				}
				
				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					
					cancellationToken.ThrowIfCancellationRequested();
					
					audioTrackInfoList.Sort((first, second) =>
						string.Compare(first.Material.CreatedAt, second.Material.CreatedAt, StringComparison.Ordinal));
					
					for (var i = audioTrackInfoList.Count - 1; i >= 0; i--)
					{
						var audioTrackInfo = audioTrackInfoList[i];
						
						AudioClipInfo audioClipInfo = null;
						
						try
						{
							audioClipInfo = await _resolver.ResolveAsync(audioTrackInfo.Material, audioTrackInfo.Bytes, cancellationToken);
						}
						catch (OperationCanceledException)
						{
							throw new OperationCanceledException(cancellationToken);
						}
						catch (Exception exception)
						{
							Debug.LogException(exception);
						}
						
						cancellationToken.ThrowIfCancellationRequested();
						
						if (audioClipInfo == null)
							continue;
						
						audioClipInfoList.Add(audioClipInfo);
					}
				}
				
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
				
				cancellationToken.ThrowIfCancellationRequested();
				
				for (var i = 0; i < audioClipInfoList.Count; i++)
				{
					var audioClipInfo = audioClipInfoList[i];
					var material = audioClipInfo.Material;
					
					try
					{
						var audioClip = AudioClip.Create(material.Name,
							audioClipInfo.SampleCount,
							audioClipInfo.ChannelCount,
							audioClipInfo.Frequency,
							false);
						
						audioClip.SetData(audioClipInfo.Data, 0);
						
						if (_options.IsManagersLogEnabled)
							Debug.Log($"Created an audio clip based on a file downloaded from \"{material.File.Url}\"");
						
						_presenter.AddTrack(material, audioClip);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
					
					await UniTask.NextFrame(cancellationToken);
				}
			}
			catch (OperationCanceledException)
			{
				Debug.LogWarning("Preloading audio materials and adding tracks have been canceled");
				return false;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return false;
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground) 
					await UniTask.SwitchToMainThread();
			}
			
			return true;
		}
		
		private async UniTask<byte[]> ConfirmMaterialAsync(AudioMaterialData material, CancellationToken cancellationToken)
		{
			var url = material.File.Url;
			
			if (string.IsNullOrEmpty(url))
			{
				Debug.LogError($"URL is null or empty when trying to download an audio file for {material}");
				return null;
			}
			
			var bytes = await DownloadFileAsync(url, material.Name);
			
			cancellationToken.ThrowIfCancellationRequested();
			
			if (bytes == null || bytes.Length == 0)
			{
				Debug.LogError($"Bytes is null or empty when trying to preload audio resource by URL: {url}");
				return null;
			}
			
			return bytes;
		}
		
		private async UniTask<byte[]> DownloadFileAsync(string url, string materialName)
		{
			if (_downloadFileCancellationTokenSource != null)
			{
				Debug.LogError($"An attempt to download a file by URL \"{url}\" was detected, which is still in progress of downloading");
				return null;
			}
			
			_downloadFileCancellationTokenSource = new CancellationTokenSourceProxy();
			
			var info = new ResourceInfo(url, ResourceCategory.Audio, materialName);
			var result = await _resources.RequestAsync<byte[]>(info, _downloadFileCancellationTokenSource.Original);
			
			if (_downloadFileCancellationTokenSource == null || _downloadFileCancellationTokenSource.IsCancellationRequested)
				return null;
            
			_downloadFileCancellationTokenSource = null;
			
			return result.Resource;
		}
		
		private async UniTaskVoid HandleAudioMessageAsync(IMessage message, Action<AudioMaterialData, AudioClip> onComplete)
		{
			try
			{
				var material = (AudioMaterialData) message.Data;
				var cancellationToken = _disposeCancellationTokenSource.Token;
				
				var bytes = await ConfirmMaterialAsync(material, cancellationToken);
				
				if (bytes == null)
					return;
				
				AudioClipInfo info;

				await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
				{
					await UniTask.SwitchToThreadPool();
					
					cancellationToken.ThrowIfCancellationRequested();
					
					info = await _resolver.ResolveAsync(material, bytes, cancellationToken);
					
					cancellationToken.ThrowIfCancellationRequested();
				}
				
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
				
				cancellationToken.ThrowIfCancellationRequested();
				
				if (info == null)
					return;
				
				var audioClip = AudioClip.Create(material.Name,
					info.SampleCount,
					info.ChannelCount,
					info.Frequency,
					false);
				
				audioClip.SetData(info.Data, 0);
				
				if (_options.IsManagersLogEnabled)
					Debug.Log($"Created an audio clip based on a file downloaded from \"{material.File.Url}\"");
				
				onComplete(material, audioClip);
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (Thread.CurrentThread.IsBackground)
					await UniTask.SwitchToMainThread();
			}
		}
		
		private async UniTaskVoid UpdateAsync()
		{
			_updateCancellationTokenSource = new CancellationTokenSource();
			
			try
			{
				var cancellationToken = _updateCancellationTokenSource.Token;
				var delayTimeSpan = TimeSpan.FromSeconds(1.0);
				
				while (_updateCancellationTokenSource != null)
				{
					if (_presenter.IsPlaying)
						PrepareToSendState();
					
					await UniTask.Delay(delayTimeSpan, cancellationToken: cancellationToken);
				}
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
		}
		
		private void PrepareToSendState()
		{
			var trackId = _presenter.CurrentTrackId;
			var looping = _presenter.IsLooping;
			var playing = _presenter.IsPlaying;
			var time = _presenter.Time;
			var timeChanged = _presenter.TimeChanged;
			
			_outgoingState.Context.SetMusicPlayerState(trackId, looping, playing, time, timeChanged);
			_outgoingState.PrepareToSendIfAllowed();
		}
		
		private void OnPlaybackCompleted(bool looped)
		{
			var messageType = looped ? MessageType.MusicPlayerTrackReset : MessageType.MusicPlayerTrackNext;
			MessageDispatcher.SendMessage(this, messageType, this, EnumMessageDelay.IMMEDIATE);
		}

		private void OnMusicPlayerLoopToggle(IMessage message)
		{
			_presenter.ToggleLoop();
			PrepareToSendState();
		}
		
		private void OnMusicPlayerPlayToggle(IMessage message)
		{
			_presenter.TogglePlay();
			PrepareToSendState();
		}
		
		private void OnMusicPlayerTrackNext(IMessage message)
		{
			_presenter.SetNextTrack();
			PrepareToSendState();
		}
		
		private void OnMusicPlayerTrackPrevious(IMessage message)
		{
			_presenter.SetPreviousTrack();
			PrepareToSendState();
		}
		
		private void OnMusicPlayerTrackSet(IMessage message)
		{
			_presenter.SetTrack((ulong) message.Data);
			
			if (!_isFirstSelection)
			{
				_presenter.TogglePlay();
				_isFirstSelection = true;
			}
			
			PrepareToSendState();
		}
		
		private void OnMusicPlayerTrackReset(IMessage message)
		{
			_presenter.ResetTrack();
			PrepareToSendState();
		}
		
		private void OnMusicPlayerTimeSet(IMessage message)
		{
			_presenter.SetTime((int) message.Data);
			PrepareToSendState();
		}
		
		private void OnAudioLoaded(IMessage message) => 
			HandleAudioMessageAsync(message, _presenter.AddTrack).Forget();
		
		private void OnAudioUnloaded(IMessage message) => 
			_presenter.RemoveTrack((AudioMaterialData) message.Data);
		
		private void OnAudioUpdated(IMessage message) => 
			HandleAudioMessageAsync(message, _presenter.UpdateTrack).Forget();
		
		private void OnPlaybackPaused(IMessage message)
		{
			if (_presenter.IsPlaying)
			{
				_presenter.TogglePlay();
				PrepareToSendState();
			}
		}
		
		private void OnApplicationInitialized(IMessage message)
		{
			MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
			
			if (_provider.TryGetGroup(AudioMixerGroupName.Common, out var group))
				_presenter.SetGroup(group);
		}
		
		private void OnAutoMaterialDataPreloadCompleted(IMessage message)
		{
			MessageDispatcher.RemoveListener(MessageType.AutoMaterialDataPreloadComplete, OnAutoMaterialDataPreloadCompleted);
			StartupAsync().Forget();
		}
	}
}