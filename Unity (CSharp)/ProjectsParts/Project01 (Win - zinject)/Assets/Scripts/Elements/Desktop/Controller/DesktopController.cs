using System;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.Desktop.Data;
using Core.Materials.Data;
using Core.Messages;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.Desktop.Presenter;
using Elements.DesktopWidgetArea.Controller;
using Elements.DesktopWidgetArea.Controller.Factory;
using Preview;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Elements.Desktop.Controller
{
	public class DesktopController : IDesktopController
	{
		private readonly IDictionary<ulong, IDesktopWidgetAreaController> _children;
		private readonly IDesktopWidgetAreaControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IOutgoingStateController _outgoingState;
		private readonly IPreviewProvider _previewProvider;
		private readonly CancellationTokenSource _unloadCancellationTokenSource;
		
		private IDesktopPresenter _presenter;

		public DesktopMaterialData Material { get; }
		public DesktopAreaMaterialData AreaMaterial { get; }

		public DesktopController(DesktopMaterialData material,
			DesktopAreaMaterialData areaMaterial,
			IDesktopWidgetAreaControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory,
			IOutgoingStateController outgoingState,
			IPreviewProvider previewProvider)
		{
			Material = material;
			AreaMaterial = areaMaterial;
			
			_children = new Dictionary<ulong, IDesktopWidgetAreaController>();
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_outgoingState = outgoingState;
			_previewProvider = previewProvider;
			_unloadCancellationTokenSource = new CancellationTokenSource();
		}
		
		public async UniTask<bool> PreloadAsync(RectTransform parent)
		{
			_presenter = _presenterFactory.Create<DesktopPresenter>(parent);
			
			if (_presenter == null)
			{
				Debug.LogError($"Failed to instantiate the DesktopPresenter by material {Material}");
				return false;
			}
			
			_presenter.Initialize(_previewProvider);
			
			if (!_presenter.SetMaterials(Material, AreaMaterial))
				return false;
			
			_presenter.AlignToParent();
			
			if (!await PreloadChildrenAsync())
				return false;
			
			var desktopId = AreaMaterial.DesktopId;
			
			if (desktopId == null)
				return false;
			
			MessageDispatcher.AddListener(MessageType.ContentAreaCreate, OnContentAreaCreated);
			MessageDispatcher.AddListener(MessageType.ContentAreaRemove, OnContentAreaRemoved);
			
			_outgoingState.Context.SetDesktopAsPreloaded(desktopId.Value);
			_outgoingState.PrepareToSendIfAllowed();
			
			return true;
		}
		
		public bool Show()
		{
			if (_presenter == null || !_presenter.Show())
				return false;
			
			MessageDispatcher.AddListener(MessageType.PreviewMakeReady, OnPreviewMakeReady);
			
			foreach (var child in _children.Values)
				child.Show();
			
			var state = _outgoingState.Context.GetDesktop();
			
			state.desktopId = AreaMaterial.DesktopId;
			state.activeDesktopId = AreaMaterial.DesktopId;
			
			_outgoingState.PrepareToSendIfAllowed();
			
			return true;
		}

		public bool Hide()
		{
			MessageDispatcher.RemoveListener(MessageType.PreviewMakeReady, OnPreviewMakeReady);
			
			foreach (var child in _children.Values)
				child.Hide();
			
			return _presenter != null && _presenter.Hide();
		}

		public void Unload()
		{
			MessageDispatcher.RemoveListener(MessageType.ContentAreaCreate, OnContentAreaCreated);
			MessageDispatcher.RemoveListener(MessageType.ContentAreaRemove, OnContentAreaRemoved);
			MessageDispatcher.RemoveListener(MessageType.PreviewMakeReady, OnPreviewMakeReady);
			
			if (!_unloadCancellationTokenSource.IsCancellationRequested)
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
			}
			
			foreach (var child in _children.Values)
				child.Unload();
			
			_children.Clear();
			
			if (_presenter != null)
			{
				_presenter.Unload();
				_presenter = null;
			}
			
			var desktopId = AreaMaterial.DesktopId;
			
			if (desktopId != null)
			{
				_outgoingState.Context.SetDesktopAsUnloaded(desktopId.Value);
				_outgoingState.PrepareToSendIfAllowed();
			}
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
					if (areaMaterial is not ContentAreaMaterialData contentAreaMaterial)
					{
						Debug.LogError($"An invalid area material {areaMaterial} is found while trying to preload a widget area. Expected material type is ContentAreaMaterialData");
						continue;
					}
					
					await UniTask.NextFrame(cancellationToken);
					
					if (!await PreloadChildAsync(contentAreaMaterial, cancellationToken))
						Debug.LogError($"Attempt to preload desktop widget area with content area material id {contentAreaMaterial.Id} is failed");
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

		private void SetChildActive(ulong areaMaterialId)
		{
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return;
			
			if (!_children.TryGetValue(areaMaterialId, out var child))
			{
				Debug.LogError($"No child found by area material id {areaMaterialId} when trying to handle a content area creation");
				return;
			}
			
			child.Show();
		}

		private async UniTask<bool> PreloadChildAsync(ContentAreaMaterialData areaMaterial, CancellationToken cancellationToken)
		{
			if (_presenter == null)
			{
				Debug.LogError($"DesktopPresenter is not yet available when trying to preload a child in the desktop with material {Material}");
				return false;
			}
			
			if (_children.ContainsKey(areaMaterial.Id))
				return true;
			
			var content = _presenter.Content;
			var child = _childControllerFactory.Create(areaMaterial);
			
			if (!await child.PreloadAsync(content))
			{
				cancellationToken.ThrowIfCancellationRequested();
				child.Unload();
				return false;
			}
			
			cancellationToken.ThrowIfCancellationRequested();

			if (!_children.TryAdd(areaMaterial.Id, child))
			{
				child.Unload();
				return false;
			}
			
			return true;
		}

		private void OnContentAreaCreated(IMessage message) => OnContentAreaCreatedAsync(message).Forget();
		
		private async UniTaskVoid OnContentAreaCreatedAsync(IMessage message)
		{
			var areaMaterial = (ContentAreaMaterialData) message.Data;
			
			if (areaMaterial.ParentId != AreaMaterial.Id)
				return;

			var cancellationToken = _unloadCancellationTokenSource.Token;
			
			try
			{
				if (!await PreloadChildAsync(areaMaterial, cancellationToken))
				{
					Debug.LogError($"Attempt to preload desktop widget area with content area material id {areaMaterial.Id} is failed");
					return;
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

			SetChildActive(areaMaterial.Id);
			
			CancellableMessageDispatcher.SendMessageDataAsync(MessageType.PreviewMakeReady, _presenter, 1f, cancellationToken).Forget();
		}
		
		private void OnContentAreaRemoved(IMessage message)
		{
			var areaMaterial = (ContentAreaMaterialData) message.Data;
			
			if (areaMaterial.ParentId != AreaMaterial.Id)
				return;
			
			var areaMaterialId = areaMaterial.Id;
			
			if (!_children.TryGetValue(areaMaterialId, out var child))
				return;
			
			child.Unload();
            
			_children.Remove(areaMaterialId);
			
			CancellableMessageDispatcher.SendMessageDataAsync(MessageType.PreviewMakeReady, _presenter, 1f, _unloadCancellationTokenSource.Token).Forget();
		}
		
		private void OnPreviewMakeReady(IMessage message)
		{
			if (_presenter is { Visible: true })
				MessageDispatcher.SendMessageData(MessageType.PreviewScreenLayersMakeReady, AreaMaterial);
		}
	}
}