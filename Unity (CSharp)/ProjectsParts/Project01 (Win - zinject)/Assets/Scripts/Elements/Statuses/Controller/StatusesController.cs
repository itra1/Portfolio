using System;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.ScreenModes;
using Core.Elements.Status.Data;
using Core.Materials.Loading.Loader;
using Core.Materials.Loading.Loader.Consts;
using Core.Materials.Loading.Loader.Info;
using Core.Materials.Storage;
using Core.Messages;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.Status.Controller;
using Elements.Status.Controller.Factory;
using Elements.Statuses.Presenter;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Elements.Statuses.Controller
{
	public class StatusesController : IStatusesController
	{
		private readonly IMaterialDataStorage _materials;
		private readonly IMaterialDataLoader _materialLoader;
		private readonly IDictionary<ulong, IStatusController> _children;
		private readonly IStatusControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly CancellationTokenSource _unloadCancellationTokenSource;
		
		private IStatusesPresenter _presenter;
		private IStatusController _activeChild;
		private bool _isChildPreloading;
		
		private bool IsChildPreloading
		{
			get => _isChildPreloading;
			set
			{
				if (_isChildPreloading != value)
				{
					_isChildPreloading = value;
					MessageDispatcher.SendMessage(this, MessageType.LoadingStateChange, value, EnumMessageDelay.IMMEDIATE);
				}
			}
		}

		public StatusMaterialData ActiveStatusMaterial => _activeChild?.Material;
		
		public StatusesController(IMaterialDataStorage materials,
			IMaterialDataLoader materialLoader,
			IStatusControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory)
		{
			_materials = materials;
			_materialLoader = materialLoader;
			_children = new Dictionary<ulong, IStatusController>();
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_unloadCancellationTokenSource = new CancellationTokenSource();
		}
		
		public bool Preload(RectTransform parent)
		{
			_presenter = _presenterFactory.Create<StatusesPresenter>(parent);
			
			if (_presenter == null)
			{
				Debug.LogError("Failed to instantiate the StatusesPresenter");
				return false;
			}
			
			_presenter.AlignToParent();
			
			MessageDispatcher.AddListener(MessageType.StatusPreload, OnStatusPreloaded);
			MessageDispatcher.AddListener(MessageType.StatusSet, OnStatusSet);
			MessageDispatcher.AddListener(MessageType.StatusUpdate, OnStatusUpdated);
			MessageDispatcher.AddListener(MessageType.StatusUnload, OnStatusUnloaded);
			
			return true;
		}
		
		public bool Show()
		{
			if (_presenter == null || !_presenter.Show())
				return false;
			
			_activeChild?.Show();
			return true;
		}
		
		public bool Hide()
		{
			_activeChild?.Hide();
			return _presenter != null && _presenter.Hide();
		}
		
		public void Unload()
		{
			MessageDispatcher.RemoveListener(MessageType.StatusPreload, OnStatusPreloaded);
			MessageDispatcher.RemoveListener(MessageType.StatusSet, OnStatusSet);
			MessageDispatcher.RemoveListener(MessageType.StatusUpdate, OnStatusUpdated);
			MessageDispatcher.RemoveListener(MessageType.StatusUnload, OnStatusUnloaded);
			
			if (!_unloadCancellationTokenSource.IsCancellationRequested)
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
			}
			
			if (IsChildPreloading)
				IsChildPreloading = false;
			
			foreach (var child in _children.Values)
				child.Unload();
			
			_children.Clear();
			
			_activeChild = null;
			
			if (_presenter != null)
			{
				_presenter.Unload();
				_presenter = null;
			}
		}
		
		private async UniTask<bool> PreloadChildAsync(ulong materialId)
		{
			if (_unloadCancellationTokenSource.IsCancellationRequested || _presenter == null)
				return false;
			
			if (_children.ContainsKey(materialId))
				return true;
			
			if (IsChildPreloading)
			{
				try
				{
					await UniTask.WaitWhile(() => IsChildPreloading, cancellationToken: _unloadCancellationTokenSource.Token);
				}
				catch (OperationCanceledException)
				{
					return false;
				}
				
				return _children.ContainsKey(materialId);
			}
			
			IsChildPreloading = true;
			
			var result = FindMaterials(materialId);
			
			if (result == null)
			{
				result = await LoadMaterialsAsync(materialId);
				
				if (result == null)
				{
					IsChildPreloading = false;
					return false;
				}
			}
			
			var (material, areaMaterial) = result.Value;
			var content = _presenter.Content;
			var child = _childControllerFactory.Create(material, areaMaterial);
			
			if (!await child.PreloadAsync(content) || _unloadCancellationTokenSource.IsCancellationRequested)
			{
				IsChildPreloading = false;
				return false;
			}
			
			_children.Add(materialId, child);
			
			IsChildPreloading = false;
			return true;
		}
		
		private void ConfirmChildPlaylists(ulong materialId)
		{
			if (!_children.TryGetValue(materialId, out var child))
				return;
			
			if (_activeChild == null || child != _activeChild)
				return;
			
			child.ConfirmPlaylists(ActiveStatusMaterial.PlaylistColumns);
		}
		
		private void SetChildActive(ulong materialId)
		{
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return;
			
			if (!_children.TryGetValue(materialId, out var child))
			{
				_activeChild?.Hide();
				return;
			}
			
			if (child == _activeChild)
			{
				MessageDispatcher.SendMessageData(MessageType.ScreenSelect, ScreenMode.Status);
				
				_activeChild.Show();
			}
			else
			{
				var previousActiveChild = _activeChild;
				
				_activeChild = child;
				
				MessageDispatcher.SendMessageData(MessageType.ScreenSelect, ScreenMode.Status);
				
				_activeChild.Show();
				
				previousActiveChild?.Hide();
			}
		}
		
		private void UnloadChild(ulong materialId)
		{
			if (_unloadCancellationTokenSource.IsCancellationRequested) 
				return;
			
			if (!_children.TryGetValue(materialId, out var child))
				return;
			
			child.Unload();
			
			if (child == _activeChild)
				_activeChild = null;
			
			_children.Remove(materialId);
		}
		
		private (StatusMaterialData, StatusAreaMaterialData)? FindMaterials(ulong materialId)
		{
			var material = _materials.Get<StatusMaterialData>(materialId);
			
			if (material == null)
				return default;
			
			var areaMaterials = _materials.GetList<StatusAreaMaterialData>();
			
			StatusAreaMaterialData areaMaterial = null;
			
			for (var i = areaMaterials.Count - 1; i >= 0; i--)
			{
				var am = areaMaterials[i];
				
				if (am == null || am.StatusId != materialId)
					continue;
				
				areaMaterial = am;
				break;
			}
			
			return areaMaterial == null ? default : (material, areaMaterial);
		}
		
		private async UniTask<(StatusMaterialData, StatusAreaMaterialData)?> LoadMaterialsAsync(ulong materialId)
		{
			var info = new MaterialDataLoadingInfo(typeof(StatusMaterialData), materialId, UrlPostfix.UnityOptimized);
			
			var result = await _materialLoader.LoadAsync(info);
			
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return default;
			
			if (!result.Success)
			{
				Debug.LogError($"Status material with id {materialId} was not loaded");
				return default;
			}
			
			if (!result.TryGetFirstMaterial<StatusMaterialData>(out var material))
			{
				Debug.LogError($"No status material was found in the loaded list of materials by requested material id {materialId}");
				return default;
			}
			
			if (!result.TryGetFirstMaterial<StatusAreaMaterialData>(out var areaMaterial))
			{
				Debug.LogError($"No status area material was found in the loaded list of materials by requested material id {materialId} ");
				return default;
			}
			
			return (material, areaMaterial);
		}
		
		private void OnStatusPreloaded(IMessage message) => PreloadChildAsync((ulong) message.Data).Forget();
		private void OnStatusSet(IMessage message) => OnStatusSetAsync(message).Forget();
		private void OnStatusUpdated(IMessage message) => OnStatusUpdateAsync(message).Forget();
		private void OnStatusUnloaded(IMessage message) => OnStatusUnloadedAsync(message).Forget();
		
		private async UniTaskVoid OnStatusSetAsync(IMessage message)
		{
			var materialId = (ulong) message.Data;
			
			var isPreloadingSuccessfully = false;
			
			if (await PreloadChildAsync(materialId))
			{
				SetChildActive(materialId);
				ConfirmChildPlaylists(materialId);
				
				isPreloadingSuccessfully = true;
			}
			else
			{
				Debug.LogError($"Attempt to preload status with material id {materialId} is failed");
			}
			
			MessageDispatcher.SendMessageData(MessageType.StatusSetCompleted, isPreloadingSuccessfully);
		}

		private async UniTaskVoid OnStatusUpdateAsync(IMessage message)
		{
			var materialId = (ulong) message.Data;
			
			if (IsChildPreloading)
			{
				try
				{
					await UniTask.WaitWhile(() => IsChildPreloading, cancellationToken: _unloadCancellationTokenSource.Token);
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
			}

			ConfirmChildPlaylists(materialId);
		}
		
		private async UniTaskVoid OnStatusUnloadedAsync(IMessage message)
		{
			var materialId = (ulong) message.Data;

			if (IsChildPreloading)
			{
				try
				{
					await UniTask.WaitWhile(() => IsChildPreloading, cancellationToken: _unloadCancellationTokenSource.Token);
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
			}
			
			UnloadChild(materialId);
		}
	}
}