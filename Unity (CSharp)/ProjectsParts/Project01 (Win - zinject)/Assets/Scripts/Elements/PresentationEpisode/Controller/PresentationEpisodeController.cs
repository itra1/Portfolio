using System;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.PresentationEpisode.Data;
using Core.Materials.Data;
using Core.Materials.Loading.Loader;
using Core.Materials.Loading.Loader.Consts;
using Core.Materials.Loading.Loader.Info;
using Core.Messages;
using Core.Network.Socket.Packets.Incoming.Actions;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.Presentation.Controller.CloneAlias;
using Elements.PresentationEpisode.Presenter;
using Elements.PresentationEpisodeScreen.Controller;
using Elements.PresentationEpisodeScreen.Controller.Factory;
using UnityEngine;

namespace Elements.PresentationEpisode.Controller
{
	public class PresentationEpisodeController : IPresentationEpisodeController
	{
		private readonly IMaterialDataLoader _materialLoader;
		private readonly IPresentationCloneAliasStorage _cloneAliasStorage;
		private readonly IDictionary<ulong, IPresentationEpisodeScreenController> _children;
		private readonly IPresentationEpisodeScreenControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IOutgoingStateController _outgoingState;
		private readonly CancellationTokenSource _unloadCancellationTokenSource;
		
		private IPresentationEpisodePresenter _presenter;
		
		public PresentationEpisodeMaterialData Material { get; }
		public PresentationEpisodeAreaMaterialData AreaMaterial { get; }
		
		public PresentationEpisodeController(PresentationEpisodeMaterialData material, 
			PresentationEpisodeAreaMaterialData areaMaterial,
			IMaterialDataLoader materialLoader,
			IPresentationCloneAliasStorage cloneAliasStorage,
			IPresentationEpisodeScreenControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory,
			IOutgoingStateController outgoingState)
		{
			Material = material;
			AreaMaterial = areaMaterial;
			
			_materialLoader = materialLoader;
			_cloneAliasStorage = cloneAliasStorage;
			_children = new Dictionary<ulong, IPresentationEpisodeScreenController>(3);
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_outgoingState = outgoingState;
			_unloadCancellationTokenSource = new CancellationTokenSource();
		}
		
		public async UniTask<bool> PreloadAsync(RectTransform parent)
		{
			_presenter = _presenterFactory.Create<PresentationEpisodePresenter>(parent);
			
			if (_presenter == null)
			{
				Debug.LogError($"Failed to instantiate the PresentationEpisodePresenter by material {Material}");
				return false;
			}
			
			if (!_presenter.SetMaterials(Material, AreaMaterial))
				return false;
			
			_presenter.AlignToParent();

			if (!await PreloadChildrenAsync())
				return false;
			
			MessageDispatcher.AddListener(MessageType.PresentationEpisodeUpdate, OnPresentationEpisodeUpdated);
			MessageDispatcher.AddListener(MessageType.WindowMaterialAction, OnWindowMaterialAction);
			
			return true;
		}

		public bool Show()
		{
			if (_presenter == null || !_presenter.Show())
				return false;
			
			foreach (var child in _children.Values)
				child.Show();
			
			_outgoingState.Context.GetPresentation().episodeId = AreaMaterial.EpisodeId;
			_outgoingState.PrepareToSendIfAllowed();
			
			return true;
		}
		
		public bool Hide()
		{
			foreach (var child in _children.Values)
				child.Hide();
			
			return _presenter != null && _presenter.Hide();
		}

		public void Unload()
		{
			MessageDispatcher.RemoveListener(MessageType.PresentationEpisodeUpdate, OnPresentationEpisodeUpdated);
			MessageDispatcher.RemoveListener(MessageType.WindowMaterialAction, OnWindowMaterialAction);
			
			if (!_unloadCancellationTokenSource.IsCancellationRequested)
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
			}
			
			foreach (var child in _children.Values)
				child.Unload();
			
			_children.Clear();
			
			if (_presenter == null) 
				return;
			
			_presenter.Unload();
			_presenter = null;
		}

		private async UniTask<bool> PreloadChildrenAsync()
		{
			try
			{
				var cancellationToken = _unloadCancellationTokenSource.Token;
				
				cancellationToken.ThrowIfCancellationRequested();
				
				var content = _presenter.Content;
				var areaMaterials = AreaMaterial.Children;
				
				foreach (var areaMaterial in areaMaterials)
				{
					if (areaMaterial is not ContentAreaMaterialData contentAreaMaterial)
					{
						Debug.LogError($"An invalid area material {areaMaterial} is found while trying to preload a presentation episode screen. Expected material type is ContentAreaMaterialData");
						continue;
					}
					
					await UniTask.NextFrame(cancellationToken);
					
					if (!await PreloadChildAsync(content, contentAreaMaterial, cancellationToken))
						Debug.LogError($"Attempt to preload presentation episode screen with content area material id {contentAreaMaterial.Id} is failed");
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
		
		private async UniTask<bool> PreloadChildAsync(RectTransform parent, ContentAreaMaterialData areaMaterial, CancellationToken cancellationToken)
		{
			var areaMaterialId = areaMaterial.Id;
			
			if (_children.ContainsKey(areaMaterialId))
				return true;
			
			var child = _childControllerFactory.Create(AreaMaterial.ParentId, areaMaterial, _cloneAliasStorage);
			
			if (!await child.PreloadAsync(parent))
			{
				cancellationToken.ThrowIfCancellationRequested();
				child.Unload();
				return false;
			}
			
			cancellationToken.ThrowIfCancellationRequested();
			
			if (!_children.TryAdd(areaMaterialId, child))
			{
				child.Unload();
				return false;
			}
			
			child.FindOrCreateState(AreaMaterial.Id);
			
			if (_presenter.Visible)
				child.Show();
			
			return true;
		}

		private void OnPresentationEpisodeUpdated(IMessage message) => OnPresentationEpisodeUpdatedAsync(message).Forget();
		
		private async UniTaskVoid OnPresentationEpisodeUpdatedAsync(IMessage message)
		{
			var materialId = Material.Id;
			
			if (materialId != ((PresentationEpisodeMaterialData) message.Data).Id)
				return;
			
			var info = new MaterialDataLoadingInfo(typeof(PresentationEpisodeMaterialData), materialId, UrlPostfix.UnityOptimized);
			
			var result = await _materialLoader.LoadAsync(info);
			
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return;
			
			if (!result.Success)
			{
				Debug.LogError($"Presentation episode material with id {materialId} was not loaded");
				return;
			}
			
			var childIds = AreaMaterial.ChildIds;
			
			IList<ulong> childIdsToRemove = null;
			
			foreach (var childId in _children.Keys)
			{
				if (childIds.Contains(childId)) 
					continue;
				
				(childIdsToRemove ??= new List<ulong>()).Add(childId);
			}
			
			if (childIdsToRemove != null)
			{
				foreach (var childId in childIdsToRemove)
				{
					_children[childId].Unload();
					_children.Remove(childId);
				}
			}
			
			await PreloadChildrenAsync();
		}

		private void OnWindowMaterialAction(IMessage message)
		{
			var action = (WindowMaterialAction) message.Data;
			
			var episodeId = action.EpisodeId;
			
			if (episodeId == null || AreaMaterial.EpisodeId != episodeId.Value)
				return;
			
			var areaId = action.AreaId;
			
			if (areaId == null)
				return;
			
			if (!_children.TryGetValue(areaId.Value, out var child))
			{
				Debug.LogError($"No child found by area material id {areaId} when trying to handle a window material action");
				return;
			}
			
			child.PerformAction(action.Alias, action.MaterialId);
		}
	}
}