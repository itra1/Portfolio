using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.Presentation.Data;
using Core.Elements.PresentationEpisode.Data;
using Core.Materials.Loading.Loader;
using Core.Materials.Loading.Loader.Consts;
using Core.Materials.Loading.Loader.Info;
using Core.Materials.Storage;
using Core.Messages;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.Presentation.Controller.CloneAlias;
using Elements.Presentation.Presenter;
using Elements.PresentationEpisode.Controller;
using Elements.PresentationEpisode.Controller.Factory;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Elements.Presentation.Controller
{
	public class PresentationController : IPresentationController
	{
		private readonly IMaterialDataStorage _materials;
		private readonly IMaterialDataLoader _materialLoader;
		private readonly IPresentationCloneAliasStorage _cloneAliasStorage;
		private readonly IDictionary<ulong, IPresentationEpisodeController> _children;
		private readonly IPresentationEpisodeControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IOutgoingStateController _outgoingState;
		private readonly CancellationTokenSource _unloadCancellationTokenSource;
		
		private IPresentationPresenter _presenter;
		private IPresentationEpisodeController _activeChild;
		
		public PresentationMaterialData Material { get; }
		public PresentationAreaMaterialData AreaMaterial { get; }
		public PresentationEpisodeMaterialData ActiveEpisodeMaterial => _activeChild?.Material;
		public PresentationEpisodeAreaMaterialData ActiveEpisodeAreaMaterial => _activeChild?.AreaMaterial;
		
		public PresentationController(PresentationMaterialData material,
			PresentationAreaMaterialData areaMaterial,
			IMaterialDataStorage materials,
			IMaterialDataLoader materialLoader,
			IPresentationEpisodeControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory,
			IOutgoingStateController outgoingState)
		{
			Material = material;
			AreaMaterial = areaMaterial;
			
			_materials = materials;
			_materialLoader = materialLoader;
			_cloneAliasStorage = new PresentationCloneAliasStorage();
			_children = new Dictionary<ulong, IPresentationEpisodeController>();
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_outgoingState = outgoingState;
			_unloadCancellationTokenSource = new CancellationTokenSource();
		}
		
		public async UniTask<bool> PreloadAsync(RectTransform parent)
		{
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return false;
			
			_presenter = _presenterFactory.Create<PresentationPresenter>(parent);
			
			if (_presenter == null)
			{
				Debug.LogError($"Failed to instantiate the PresentationPresenter by material {Material}");
				return false;
			}

			if (!_presenter.SetMaterials(Material, AreaMaterial))
				return false;
			
			_presenter.AlignToParent();
			
			if (!await PreloadChildrenAsync())
				return false;
			
			_outgoingState.Context.SetPresentationAsPreloaded(AreaMaterial.PresentationId);
			_outgoingState.PrepareToSendIfAllowed();
			
			return true;
		}
		
		public bool Show()
		{
			if (_presenter == null || !_presenter.Show())
				return false;
			
			if (_activeChild != null)
				_activeChild.Show();
			else if (_children.Count > 0)
				SetChildActive(_children.First().Key);
			
			_outgoingState.Context.GetPresentation().presentationId = AreaMaterial.PresentationId;
			_outgoingState.PrepareToSendIfAllowed();
			
			MessageDispatcher.AddListener(MessageType.PresentationEpisodePreload, OnPresentationEpisodePreloaded);
			MessageDispatcher.AddListener(MessageType.PresentationEpisodeUnload, OnPresentationEpisodeUnloaded);
			MessageDispatcher.AddListener(MessageType.PreviewScreenModeMakeReady, OnPreviewMakeReady);
			
			return true;
		}
		
		public bool Hide()
		{
			MessageDispatcher.RemoveListener(MessageType.PreviewScreenModeMakeReady, OnPreviewMakeReady);
			MessageDispatcher.RemoveListener(MessageType.PresentationEpisodePreload, OnPresentationEpisodePreloaded);
			MessageDispatcher.RemoveListener(MessageType.PresentationEpisodeUnload, OnPresentationEpisodeUnloaded);
			
			_activeChild?.Hide();
			
			return _presenter != null && _presenter.Hide();
		}

		public void Unload()
		{
			MessageDispatcher.RemoveListener(MessageType.PreviewScreenModeMakeReady, OnPreviewMakeReady);
			MessageDispatcher.RemoveListener(MessageType.PresentationEpisodePreload, OnPresentationEpisodePreloaded);
			MessageDispatcher.RemoveListener(MessageType.PresentationEpisodeUnload, OnPresentationEpisodeUnloaded);
			
			if (!_unloadCancellationTokenSource.IsCancellationRequested)
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
			}
			
			foreach (var child in _children.Values)
				child.Unload();
			
			_children.Clear();
			
			_activeChild = null;
			
			_cloneAliasStorage.Clear();
			
			if (_presenter != null)
			{
				_presenter.Unload();
				_presenter = null;
			}
			
			_outgoingState.Context.SetPresentationAsUnloaded(AreaMaterial.PresentationId);
			_outgoingState.PrepareToSendIfAllowed();
		}
		
		public async UniTask<bool> SetChildActiveAsync(ulong materialId)
		{
			if (!await PreloadChildAsync(materialId))
			{
				Debug.LogError($"Attempt to preload presentation episode with material id {materialId} is failed");
				return false;
			}
			
			SetChildActive(materialId);
			return true;
		}
		
		private async UniTask<bool> PreloadChildrenAsync()
		{
			try
			{
				var cancellationToken = _unloadCancellationTokenSource.Token;
				
				cancellationToken.ThrowIfCancellationRequested();
				
				var areaMaterials = AreaMaterial.Children;
				
				foreach (var areaMaterial in areaMaterials)
				{
					if (areaMaterial is not PresentationEpisodeAreaMaterialData episodeAreaMaterial)
					{
						Debug.LogError($"An invalid area material {areaMaterial} is found while trying to preload a presentation episode. Expected material type is PresentationEpisodeAreaMaterialData");
						continue;
					}
					
					await UniTask.NextFrame(cancellationToken);
					
					var materialId = episodeAreaMaterial.EpisodeId;
					
					if (!await PreloadChildAsync(materialId))
						Debug.LogError($"Attempt to preload presentation episode with material id {materialId} is failed");
				}
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
			
			return true;
		}
		
		private async UniTask<bool> PreloadChildAsync(ulong materialId)
		{
			if (_unloadCancellationTokenSource.IsCancellationRequested || _presenter == null)
				return false;
			
			if (_children.ContainsKey(materialId))
				return true;
			
			var result = FindMaterials(materialId);
			
			if (result == null)
			{
				Debug.LogError($"Presentation episode material and presentation episode area material have not been found by episode id: {materialId}");
				return false;
			}
			
			var (material, areaMaterial) = result.Value;
			var content = _presenter.Content;
			var child = _childControllerFactory.Create(material, areaMaterial, _cloneAliasStorage);
			
			if (!await child.PreloadAsync(content))
			{
				if (!_unloadCancellationTokenSource.IsCancellationRequested)
					child.Unload();
				
				return false;
			}
			
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return false;
			
			if (!_children.TryAdd(materialId, child))
			{
				child.Unload();
				return false;
			}
			
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
				return;
			
			var previousActiveChild = _activeChild;
			
			_activeChild = child;
			
			_activeChild.Show();
			
			previousActiveChild?.Hide();
		}
		
		private (PresentationEpisodeMaterialData, PresentationEpisodeAreaMaterialData)? FindMaterials(ulong materialId)
		{
			var material = _materials.Get<PresentationEpisodeMaterialData>(materialId);
			
			if (material == null)
				return null;
			
			var areaMaterials = _materials.GetList<PresentationEpisodeAreaMaterialData>();
			
			PresentationEpisodeAreaMaterialData areaMaterial = null;
			
			for (var i = areaMaterials.Count - 1; i >= 0; i--)
			{
				var am = areaMaterials[i];
				
				if (am == null || am.EpisodeId != materialId)
					continue;
				
				areaMaterial = am;
				break;
			}
			
			if (areaMaterial == null)
				return null;
			
			return (material, areaMaterial);
		}
		
		private void OnPreviewMakeReady(IMessage message)
		{
			if (_presenter is { Visible: true })
				MessageDispatcher.SendMessageData(MessageType.PreviewScreenLayersMakeReady, AreaMaterial);
		}

		private void OnPresentationEpisodePreloaded(IMessage message) => 
			OnPresentationEpisodePreloadedAsync(message).Forget();
		
		private async UniTaskVoid OnPresentationEpisodePreloadedAsync(IMessage message)
		{
			var material = (PresentationEpisodeMaterialData) message.Data;
			var materialId = material.Id;
			
			var info = new MaterialDataLoadingInfo(typeof(PresentationEpisodeMaterialData), materialId, UrlPostfix.UnityOptimized);
			
			var result = await _materialLoader.LoadAsync(info);
			
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return;
			
			if (!result.Success)
			{
				Debug.LogError($"Presentation episode material with id {materialId} was not loaded");
				return;
			}
			
			if (!await PreloadChildAsync(materialId))
			{
				Debug.LogError($"Attempt to preload presentation episode with material id {materialId} is failed");
				return;
			}
			
			if (!_children.TryGetValue(materialId, out var child))
				return;
			
			var areaMaterial = child.AreaMaterial;
			var areaMaterialId = areaMaterial.Id;
			
			var childIds = AreaMaterial.ChildIds;
			var children = AreaMaterial.Children;
			
			if (!childIds.Contains(areaMaterialId))
				childIds.Add(areaMaterialId);
			
			if (!children.Contains(areaMaterial))
				children.Add(areaMaterial);
			
			if (_activeChild == null)
				SetChildActive(materialId);
		}

		private void OnPresentationEpisodeUnloaded(IMessage message)
		{
			var material = (PresentationEpisodeMaterialData) message.Data;
			var materialId = material.Id;
			
			if (!_children.TryGetValue(materialId, out var child))
				return;
			
			var areaMaterial = child.AreaMaterial;
			
			child.Unload();
			
			_children.Remove(materialId);
			
			AreaMaterial.ChildIds.Remove(areaMaterial.Id);
			AreaMaterial.Children.Remove(areaMaterial);
		}
	}
}