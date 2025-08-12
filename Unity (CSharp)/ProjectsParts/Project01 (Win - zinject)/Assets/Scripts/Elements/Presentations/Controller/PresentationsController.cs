using System;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.Presentation.Data;
using Core.Elements.PresentationEpisode.Data;
using Core.Elements.ScreenModes;
using Core.Materials.Loading.Loader;
using Core.Materials.Loading.Loader.Consts;
using Core.Materials.Loading.Loader.Info;
using Core.Materials.Storage;
using Core.Messages;
using Core.Network.Socket.Packets.Incoming.Actions.Data;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.Presentation.Controller;
using Elements.Presentation.Controller.Factory;
using Elements.Presentations.Presenter;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Elements.Presentations.Controller
{
	public class PresentationsController : IPresentationsController
	{
		private readonly IMaterialDataStorage _materials;
		private readonly IMaterialDataLoader _materialLoader;
		private readonly IDictionary<ulong, IPresentationController> _children;
		private readonly IPresentationControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly CancellationTokenSource _unloadCancellationTokenSource;

		private IPresentationsPresenter _presenter;
		private IPresentationController _activeChild;
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
		
		public PresentationMaterialData ActivePresentationMaterial => _activeChild?.Material;
		public PresentationAreaMaterialData ActivePresentationAreaMaterial => _activeChild?.AreaMaterial;
		public PresentationEpisodeMaterialData ActivePresentationEpisodeMaterial => _activeChild?.ActiveEpisodeMaterial;
		public PresentationEpisodeAreaMaterialData ActivePresentationAreaEpisodeMaterial => _activeChild?.ActiveEpisodeAreaMaterial;

		public PresentationsController(IMaterialDataStorage materials,
			IMaterialDataLoader materialLoader,
			IPresentationControllerFactory childControllerFactory, 
			IPresenterFactory presenterFactory)
		{
			_materials = materials;
			_materialLoader = materialLoader;
			_children = new Dictionary<ulong, IPresentationController>();
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_unloadCancellationTokenSource = new CancellationTokenSource();
		}

		public bool Preload(RectTransform parent)
		{
			_presenter = _presenterFactory.Create<PresentationsPresenter>(parent);

			if (_presenter == null)
			{
				Debug.LogError("Failed to instantiate the PresentationsPresenter");
				return false;
			}
			
			_presenter.AlignToParent();
			
			MessageDispatcher.AddListener(MessageType.PresentationPreload, OnPresentationPreloaded);
			MessageDispatcher.AddListener(MessageType.PresentationUnload, OnPresentationUnloaded);
			MessageDispatcher.AddListener(MessageType.PresentationSet, OnPresentationSet);
			MessageDispatcher.AddListener(MessageType.PresentationEpisodeSet, OnPresentationEpisodeSet);

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
			MessageDispatcher.RemoveListener(MessageType.PresentationPreload, OnPresentationPreloaded);
			MessageDispatcher.RemoveListener(MessageType.PresentationUnload, OnPresentationUnloaded);
			MessageDispatcher.RemoveListener(MessageType.PresentationSet, OnPresentationSet);
			MessageDispatcher.RemoveListener(MessageType.PresentationEpisodeSet, OnPresentationEpisodeSet);
			
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
				catch (Exception exception)
				{
					Debug.LogException(exception);
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
				MessageDispatcher.SendMessageData(MessageType.ScreenSelect, ScreenMode.Presentation);
				
				_activeChild.Show();
			}
			else
			{
				var previousActiveChild = _activeChild;
				
				_activeChild = child;
				
				MessageDispatcher.SendMessageData(MessageType.ScreenSelect, ScreenMode.Presentation);
				
				_activeChild.Show();
				
				previousActiveChild?.Hide();
			}
		}

		private void UnloadChild(ulong materialId)
		{
			if (_unloadCancellationTokenSource.IsCancellationRequested) 
				return;
			
			if (!_children.Remove(materialId, out var child))
				return;
			
			if (child == _activeChild)
				_activeChild = null;
			
			child.Unload();
		}
		
		private (PresentationMaterialData, PresentationAreaMaterialData)? FindMaterials(ulong materialId)
		{
			var material = _materials.Get<PresentationMaterialData>(materialId);
			
			if (material == null)
				return default;
			
			var areaMaterials = _materials.GetList<PresentationAreaMaterialData>();
			
			PresentationAreaMaterialData areaMaterial = null;
			
			for (var i = areaMaterials.Count - 1; i >= 0; i--)
			{
				var am = areaMaterials[i];
				
				if (am == null || am.PresentationId != materialId)
					continue;
				
				areaMaterial = am;
				break;
			}
			
			return areaMaterial == null ? default : (material, areaMaterial);
		}
		
		private async UniTask<(PresentationMaterialData, PresentationAreaMaterialData)?> LoadMaterialsAsync(ulong materialId)
		{
			var info = new MaterialDataLoadingInfo(typeof(PresentationMaterialData), materialId, UrlPostfix.UnityOptimized);
			
			var result = await _materialLoader.LoadAsync(info);
			
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return default;

			if (!result.Success)
			{
				Debug.LogError($"Presentation material with id {materialId} was not loaded");
				return default;
			}
			
			if (!result.TryGetFirstMaterial<PresentationMaterialData>(out var material))
			{
				Debug.LogError($"No presentation material was found in the loaded list of materials by requested material id {materialId}");
				return default;
			}
			
			if (!result.TryGetFirstMaterial<PresentationAreaMaterialData>(out var areaMaterial))
			{
				Debug.LogError($"No presentation area material was found in the loaded list of materials by requested material id {materialId} ");
				return default;
			}
			
			return (material, areaMaterial);
		}

		private void OnPresentationPreloaded(IMessage message) => PreloadChildAsync((ulong) message.Data).Forget();
		private void OnPresentationSet(IMessage message) => OnPresentationSetAsync(message).Forget();
		private void OnPresentationEpisodeSet(IMessage message) => OnPresentationEpisodeSetAsync(message).Forget();
		private void OnPresentationUnloaded(IMessage message) => OnPresentationUnloadedAsync(message).Forget();
		
		private async UniTaskVoid OnPresentationSetAsync(IMessage message)
		{
			var materialId = (ulong) message.Data;
			
			if (!await PreloadChildAsync(materialId))
			{
				Debug.LogError($"Attempt to preload presentation with material id {materialId} is failed");
				return;
			}
			
			SetChildActive(materialId);
		}
		
		private async UniTaskVoid OnPresentationEpisodeSetAsync(IMessage message)
		{
			var data = (PresentationData) message.Data;
			var presentationId = data.PresentationId;

			if (!await PreloadChildAsync(presentationId))
			{
				Debug.LogError($"Attempt to preload presentation with material id {presentationId} is failed");
				return;
			}
			
			SetChildActive(presentationId);
			
			_activeChild?.SetChildActiveAsync(data.EpisodeId).Forget();
		}
		
		private async UniTaskVoid OnPresentationUnloadedAsync(IMessage message)
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