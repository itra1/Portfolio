using System;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.Desktop.Data;
using Core.Elements.ScreenModes;
using Core.Materials.Loading.Loader;
using Core.Materials.Loading.Loader.Consts;
using Core.Materials.Loading.Loader.Info;
using Core.Materials.Storage;
using Core.Messages;
using Core.Options;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.Desktop.Controller;
using Elements.Desktop.Controller.Factory;
using Elements.Desktops.Presenter;
using Elements.Widgets.Common.Controller;
using UnityEngine;
using Zenject;

namespace Elements.Desktops.Controller
{
	public class DesktopsController : IDesktopsController
	{
		private readonly IMaterialDataStorage _materials;
		private readonly IMaterialDataLoader _materialLoader;
		private readonly IDictionary<ulong, IDesktopController> _children;
		private readonly IDesktopControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IWidgetsUpdateController _widgetsUpdateController;
		
		private readonly CancellationTokenSource _unloadCancellationTokenSource;
		
		private IDesktopsPresenter _presenter;
		private IDesktopController _activeChild;
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
		
		public DesktopMaterialData ActiveDesktopMaterial => _activeChild?.Material;
		public DesktopAreaMaterialData ActiveDesktopAreaMaterial => _activeChild?.AreaMaterial;
		
		public DesktopsController(DiContainer container,
			IApplicationOptions options,
			IMaterialDataStorage materials,
			IMaterialDataLoader materialLoader,
			IDesktopControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory)
		{
			_materials = materials;
			_materialLoader = materialLoader;
			_children = new Dictionary<ulong, IDesktopController>();
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			
			if (!options.IsSumAdaptiveModeActive)
				_widgetsUpdateController = container.Resolve<IWidgetsUpdateController>();
			
			_unloadCancellationTokenSource = new CancellationTokenSource();
		}
		
		public bool Preload(RectTransform parent)
		{
			_presenter = _presenterFactory.Create<DesktopsPresenter>(parent);

			if (_presenter == null)
			{
				Debug.LogError("Failed to instantiate the DesktopsPresenter");
				return false;
			}
			
			_presenter.AlignToParent();
			
			MessageDispatcher.AddListener(MessageType.DesktopPreload, OnDesktopPreloaded);
			MessageDispatcher.AddListener(MessageType.DesktopUnload, OnDesktopUnloaded);
			MessageDispatcher.AddListener(MessageType.DesktopSet, OnDesktopSet);

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
			MessageDispatcher.RemoveListener(MessageType.DesktopPreload, OnDesktopPreloaded);
			MessageDispatcher.RemoveListener(MessageType.DesktopUnload, OnDesktopUnloaded);
			MessageDispatcher.RemoveListener(MessageType.DesktopSet, OnDesktopSet);
			
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
			
			if (_widgetsUpdateController is { IsRunning: false })
				_widgetsUpdateController.Start();
			
			IsChildPreloading = false;
			return true;
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
				MessageDispatcher.SendMessageData(MessageType.ScreenSelect, ScreenMode.Desktop);
				
				_activeChild.Show();
			}
			else
			{
				var previousActiveChild = _activeChild;
				
				_activeChild = child;
				
				MessageDispatcher.SendMessageData(MessageType.ScreenSelect, ScreenMode.Desktop);
				
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
			
			if (_children.Count == 0 && _widgetsUpdateController != null)
				_widgetsUpdateController.Stop();
		}
		
		private (DesktopMaterialData, DesktopAreaMaterialData)? FindMaterials(ulong materialId)
		{
			var material = _materials.Get<DesktopMaterialData>(materialId);
			
			if (material == null)
				return default;
			
			var areaMaterials = _materials.GetList<DesktopAreaMaterialData>();
			
			DesktopAreaMaterialData areaMaterial = null;
					
			for (var i = areaMaterials.Count - 1; i >= 0; i--)
			{
				var am = areaMaterials[i];
				
				if (am == null || am.DesktopId != materialId)
					continue;
				
				areaMaterial = am;
				break;
			}
			
			return areaMaterial == null ? default : (material, areaMaterial);
		}
		
		private async UniTask<(DesktopMaterialData, DesktopAreaMaterialData)?> LoadMaterialsAsync(ulong materialId)
		{
			var info = new MaterialDataLoadingInfo(typeof(DesktopMaterialData), materialId, UrlPostfix.UnityOptimized);
			
			var result = await _materialLoader.LoadAsync(info);
			
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return default;
			
			if (!result.Success)
			{
				Debug.LogError($"Desktop material with id {materialId} was not loaded");
				return default;
			}
			
			if (!result.TryGetFirstMaterial<DesktopMaterialData>(out var material))
			{
				Debug.LogError($"No desktop material was found in the loaded list of materials by requested material id {materialId}");
				return default;
			}
			
			if (!result.TryGetFirstMaterial<DesktopAreaMaterialData>(out var areaMaterial))
			{
				Debug.LogError($"No desktop area material was found in the loaded list of materials by requested material id {materialId} ");
				return default;
			}
			
			return (material, areaMaterial);
		}
		
		private void OnDesktopPreloaded(IMessage message) => PreloadChildAsync((ulong) message.Data).Forget(); 
		private void OnDesktopSet(IMessage message) => OnDesktopSetAsync(message).Forget();
		private void OnDesktopUnloaded(IMessage message) => OnDesktopUnloadedAsync(message).Forget();
		
		private async UniTaskVoid OnDesktopSetAsync(IMessage message)
		{
			var materialId = (ulong) message.Data;
			
			if (!await PreloadChildAsync(materialId))
				return;
			
			SetChildActive(materialId);
		}
		
		private async UniTaskVoid OnDesktopUnloadedAsync(IMessage message)
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