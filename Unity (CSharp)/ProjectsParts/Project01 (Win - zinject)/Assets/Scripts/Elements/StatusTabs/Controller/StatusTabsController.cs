using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.ScreenModes;
using Core.Elements.StatusColumn.Data;
using Core.Materials.Data;
using Core.Messages;
using Core.Network.Api.Consts;
using Core.Network.Http;
using Core.Network.Http.Data;
using Core.Network.Socket.Packets.Incoming.Actions.Data;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Options;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.StatusTabItem.Controller;
using Elements.StatusTabItem.Controller.Factory;
using Elements.StatusTabs.Presenter;
using UnityEngine;

namespace Elements.StatusTabs.Controller
{
	public class StatusTabsController : IStatusTabsController
	{
		private readonly List<ContentAreaMaterialData> _areaMaterials;
		private readonly IApplicationOptions _options;
		private readonly IDictionary<ulong, IStatusTabItemController> _children;
		private readonly IStatusTabItemControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IHttpRequest _request;
		private readonly IOutgoingStateController _outgoingState;
		private readonly CancellationTokenSource _unloadCancellationTokenSource;
		
		private IStatusTabsPresenter _presenter;
		private IStatusTabItemController _activeChild;
		
		public StatusContentAreaMaterialData AreaMaterial { get; }
		public IReadOnlyList<ContentAreaMaterialData> AreaMaterials => _areaMaterials;
		public ContentAreaMaterialData ActiveAreaMaterial => _activeChild?.AreaMaterial;

		public int ChildrenCount => _children.Count;
		
		public event Action<ContentAreaMaterialData> StatusTabActive;
		
		public StatusTabsController(StatusContentAreaMaterialData areaMaterial,
			List<ContentAreaMaterialData> areaMaterials,
			IApplicationOptions options,
			IStatusTabItemControllerFactory childControllerFactory, 
			IPresenterFactory presenterFactory,
			IHttpRequest request,
			IOutgoingStateController outgoingState)
		{
			AreaMaterial = areaMaterial;
			
			_areaMaterials = areaMaterials;
			_options = options;
			_children = new Dictionary<ulong, IStatusTabItemController>();
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_request = request;
			_outgoingState = outgoingState;
			_unloadCancellationTokenSource = new CancellationTokenSource();
		}
		
		public async UniTask<bool> PreloadAsync(RectTransform parent)
		{
			_presenter = _presenterFactory.Create<StatusTabsPresenter>(parent);
			
			if (_presenter == null)
			{
				Debug.LogError("Failed to instantiate the StatusTabsPresenter");
				return false;
			}
			
			_presenter.AlignToParent();
			
			if (!await PreloadChildrenAsync())
				return false;
			
			MessageDispatcher.AddListener(MessageType.StatusTabActive, OnStatusTabActive);
			
			return true;
		}

		public bool Show()
		{
			if (_presenter == null || !_presenter.Show())
				return false;
			
			if (_activeChild != null)
			{
				_activeChild.Show();
			}
			else if (ChildrenCount > 0)
			{
				var statusContent = AreaMaterial.StatusContent;
				var activeMaterialId = statusContent.ActiveMaterialId;
				
				ContentAreaMaterialData areaMaterial;
				
				if (activeMaterialId == 0)
				{
					var contentOrder = statusContent.ContentOrder;
					
					if (contentOrder.Count > 0)
					{
						var activeAreaMaterialId = contentOrder.First();
						
						areaMaterial = _areaMaterials.FirstOrDefault(am => am.Id == activeAreaMaterialId);
					
						if (areaMaterial is { MaterialId: not null })
							statusContent.ActiveMaterialId = areaMaterial.MaterialId.Value;
					}
					else
					{
						areaMaterial = null;
					}
				}
				else
				{
					areaMaterial = _areaMaterials.FirstOrDefault(am => am.MaterialId == activeMaterialId);
				}
				
				if (areaMaterial == null)
					return false;
				
				SetChildActive(areaMaterial.Id);
			}
			
			return true;
		}
		
		public bool AddChild(IStatusTabItemController child)
		{
			if (_unloadCancellationTokenSource.IsCancellationRequested || _presenter == null)
				return false;
			
			var areaMaterial = child.AreaMaterial;
			
			_areaMaterials.Add(areaMaterial);
			
			child.SetParent(_presenter.Content);
			
			_children.Add(areaMaterial.Id, child);
			
			return true;
		}
		
		public async UniTask<bool> AddChildAsync(ContentAreaMaterialData areaMaterial)
		{
			if (_unloadCancellationTokenSource.IsCancellationRequested || _presenter == null)
				return false;
			
			_areaMaterials.Add(areaMaterial);
			
			var childrenCountBefore = ChildrenCount;
			
			try
			{
				if (!await PreloadChildAsync(_presenter.Content, areaMaterial, _unloadCancellationTokenSource.Token))
				{
					Debug.LogError($"Attempt to preload status tab item with area material id {areaMaterial.Id} is failed");
					return false;
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
			
			if (childrenCountBefore == 0)
				SetChildActive(areaMaterial.Id);
			
			return true;
		}
		
		public bool ContainsChild(ContentAreaMaterialData areaMaterial) =>
			_areaMaterials.Contains(areaMaterial) && _children.ContainsKey(areaMaterial.Id);
		
		public void SetChildActive(ulong areaMaterialId)
		{
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return;
			
			if (!_children.TryGetValue(areaMaterialId, out var child))
			{
				_activeChild?.Hide();
				return;
			}
			
			if (child == _activeChild)
				return;
			
			var previousActiveChild = _activeChild;
			
			_activeChild = child;
			
			MessageDispatcher.SendMessageData(MessageType.ScreenSelect, ScreenMode.Status);
			
			_activeChild.Show();
			
			StatusTabActive?.Invoke(_activeChild.AreaMaterial);
			
			previousActiveChild?.Hide();
			
			var statusContent = AreaMaterial.StatusContent;
			var column = statusContent.Column;
			
			if (column < 1)
			{
				Debug.LogError("Invalid column value when trying to set child active");
				return;
			}
			
			if (!_options.IsStateSendingAllowed)
				_request.Request(string.Format(RestApiUrl.StatusMaterialByColumnFormat, column, areaMaterialId), HttpMethodType.Patch);
			
			_outgoingState.Context.SetStatusAsActiveAt(column, statusContent.ActiveMaterialId, areaMaterialId);
			_outgoingState.PrepareToSendIfAllowed();
		}
		
		public bool RemoveChild(ContentAreaMaterialData areaMaterial, out IStatusTabItemController child)
		{
			if (_unloadCancellationTokenSource.IsCancellationRequested)
			{
				child = null;
				return false;
			}
			
			_areaMaterials.Remove(areaMaterial);
			
			var areaMaterialId = areaMaterial.Id;
			
			if (!_children.Remove(areaMaterialId, out child))
			{
				Debug.LogError($"No child found by area material id {areaMaterialId} when trying to remove it");
				return false;
			}
			
			if (child == _activeChild)
				_activeChild = null;
			
			return true;
		}
		
		public bool RemoveChild(ContentAreaMaterialData areaMaterial)
		{
			if (!RemoveChild(areaMaterial, out var child))
				return false;
			
			child.Unload();
			
			return true;
		}
		
		public void PerformAction(string actionAlias, ulong materialId)
		{
			if (ActiveAreaMaterial == null)
				return;
			
			var areaMaterialId = ActiveAreaMaterial.Id;
			
			if (!_children.TryGetValue(areaMaterialId, out var child))
			{
				Debug.LogError($"No child found by area material id {areaMaterialId} when trying to perform an action on it");
				return;
			}
			
			child.PerformAction(actionAlias, materialId);
		}
		
		public bool Hide()
		{
			_activeChild?.Hide();
			return _presenter != null && _presenter.Hide();
		}

		public void Unload()
		{
			MessageDispatcher.RemoveListener(MessageType.StatusTabActive, OnStatusTabActive);
			
			if (!_unloadCancellationTokenSource.IsCancellationRequested)
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
			}
			
			foreach (var child in _children.Values)
				child.Unload();
			
			_children.Clear();
			_areaMaterials.Clear();
			
			if (_presenter != null)
			{
				_presenter.Unload();
				_presenter = null;
			}
		}
		
		private async UniTask<bool> PreloadChildrenAsync()
		{
			try
			{
				var cancellationToken = _unloadCancellationTokenSource.Token;
				
				cancellationToken.ThrowIfCancellationRequested();
				
				var content = _presenter.Content;
				
				foreach (var areaMaterial in AreaMaterials)
				{
					await UniTask.NextFrame(cancellationToken);
					
					if (!await PreloadChildAsync(content, areaMaterial, cancellationToken))
						Debug.LogError($"Attempt to preload status tab item with area material id {areaMaterial.Id} is failed");
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
			
			var child = _childControllerFactory.Create(areaMaterial);
			
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
			
			child.FindOrCreateState(AreaMaterial.StatusContent.Id);
			
			return true;
		}
		
		private void OnStatusTabActive(IMessage message)
		{
			var data = (StatusTabData) message.Data;
			var column = data.ColumnIndex + 1;
			
			if (_options.IsSumAdaptiveModeActive && _options.SumAdaptiveModeColumn != column)
				return;
			
			var statusContent = AreaMaterial.StatusContent;
			
			if (statusContent.Id != data.ContentId || statusContent.Column != column) 
				return;
			
			SetChildActive(data.AreaId);
		}
	}
}