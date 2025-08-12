using System;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.StatusColumn.Data;
using Core.Elements.Windows.Base.Data;
using Core.Materials.Data;
using Core.Messages;
using Core.Network.Socket.Packets.Incoming.Actions;
using Core.Network.Socket.Packets.Incoming.Actions.Consts;
using Core.Network.Socket.Packets.Incoming.Actions.Data;
using Core.Options;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.StatusColumn.Controller.Helper;
using Elements.StatusColumn.Controller.Playlist;
using Elements.StatusColumn.Controller.Playlist.Factory;
using Elements.StatusColumn.Presenter;
using Elements.StatusTabItem.Controller.Buffer;
using Elements.StatusTabs.Controller;
using Elements.StatusTabs.Controller.Factory;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Elements.StatusColumn.Controller
{
	public class StatusColumnController : IStatusColumnController
	{
		private readonly IApplicationOptions _options;
		private readonly IStatusColumnMaterialsHelper _helper;
		private readonly IStatusTabItemDragAndDropBuffer _dragAndDropBuffer;
		private readonly IStatusTabsControllerFactory _tabsControllerFactory;
		private readonly IStatusColumnPlaylistFactory _playlistFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly CancellationTokenSource _unloadCancellationTokenSource;
		
		private IStatusColumnPresenter _presenter;
		private IStatusTabsController _tabsController;
		private IStatusColumnPlaylist _playlist;
		private bool _isTabsReorderingInProgress;
		
		public StatusContentAreaMaterialData AreaMaterial { get; }
		
		public StatusColumnController(StatusContentAreaMaterialData areaMaterial,
			IApplicationOptions options,
			IStatusColumnMaterialsHelper helper,
			IStatusTabItemDragAndDropBuffer dragAndDropBuffer,
			IStatusTabsControllerFactory tabsControllerFactory,
			IStatusColumnPlaylistFactory playlistFactory,
			IPresenterFactory presenterFactory)
		{
			AreaMaterial = areaMaterial;

			_options = options;
			_helper = helper;
			_dragAndDropBuffer = dragAndDropBuffer;
			_tabsControllerFactory = tabsControllerFactory;
			_playlistFactory = playlistFactory;
			_presenterFactory = presenterFactory;
			_unloadCancellationTokenSource = new CancellationTokenSource();
		}
		
		public async UniTask<bool> PreloadAsync(RectTransform parent)
		{
			_presenter = _presenterFactory.Create<StatusColumnPresenter>(parent);
			
			if (_presenter == null)
			{
				Debug.LogError($"Failed to instantiate the StatusColumnPresenter by material {AreaMaterial.StatusContent}");
				return false;
			}
			
			if (!_presenter.SetMaterial(AreaMaterial))
				return false;
			
			_presenter.AlignToParent();
			
			if (!await PreloadChildrenAsync())
				return false;
			
			MessageDispatcher.AddListener(MessageType.WindowMaterialAction, OnWindowMaterialAction);
			MessageDispatcher.AddListener(MessageType.ContentAreaCreate, OnContentAreaCreated);
			MessageDispatcher.AddListener(MessageType.ContentAreaRemove, OnContentAreaRemoved);
			MessageDispatcher.AddListener(MessageType.ContentAreaUpdate, OnContentAreaUpdated);
			MessageDispatcher.AddListener(MessageType.StatusOrderUpdate, OnStatusOrderUpdated);
			MessageDispatcher.AddListener(MessageType.StatusContentAreaCreateMany, OnStatusContentAreaCreateMany);
			MessageDispatcher.AddListener(MessageType.StatusTabItemDragAndDrop, OnStatusTabItemDraggedAndDropped);
			
			return true;
		}

		public bool Show()
		{
			if (_presenter == null || !_presenter.Show())
				return false;
			
			if (_tabsController != null)
                _tabsController.Show();
			else 
				Debug.LogError($"StatusTabsController is null when trying to show a StatusColumnController with area material: {AreaMaterial}");
            
			return true;
		}
		
		public void EnablePlaylist()
		{
			_playlist ??= _playlistFactory.Create(AreaMaterial.StatusContent, _tabsController);

			if (_playlist is { IsPlaying: false })
				_playlist.Play();
		}
		
		public void AttemptToDisablePlaylist()
		{
			if (_playlist == null)
				return;
			
			_playlist.Dispose();
			_playlist = null;
		}
		
		public bool Hide()
		{
			AttemptToDisablePlaylist();
			
			if (_tabsController != null)
				_tabsController.Hide();
			else 
				Debug.LogError($"StatusTabsController is null when trying to hide a StatusColumnController with area material: {AreaMaterial}");
			
			return _presenter != null && _presenter.Hide();
		}
		
		public void Unload()
		{
			MessageDispatcher.RemoveListener(MessageType.WindowMaterialAction, OnWindowMaterialAction);
			MessageDispatcher.RemoveListener(MessageType.ContentAreaCreate, OnContentAreaCreated);
			MessageDispatcher.RemoveListener(MessageType.ContentAreaRemove, OnContentAreaRemoved);
			MessageDispatcher.RemoveListener(MessageType.ContentAreaUpdate, OnContentAreaUpdated);
			MessageDispatcher.RemoveListener(MessageType.StatusOrderUpdate, OnStatusOrderUpdated);
			MessageDispatcher.RemoveListener(MessageType.StatusContentAreaCreateMany, OnStatusContentAreaCreateMany);
			MessageDispatcher.RemoveListener(MessageType.StatusTabItemDragAndDrop, OnStatusTabItemDraggedAndDropped);
			
			if (!_unloadCancellationTokenSource.IsCancellationRequested)
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
			}
			
			AttemptToDisablePlaylist();
			
			if (_tabsController != null)
			{
				_tabsController.StatusTabActive -= OnStatusTabActive;
				_tabsController.Unload();
				_tabsController = null;
			}
			
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
				
				var statusContent = AreaMaterial.StatusContent;
				
				var orderedAreaMaterials = new List<ContentAreaMaterialData>();
				var orderedMaterials = new List<WindowMaterialData>();
				
				if (!await _helper.TryFindOrderedMaterialsAsync(statusContent.ContentOrder, orderedAreaMaterials, orderedMaterials, cancellationToken))
					return false;
				
				var content = _presenter.Content;
				
				_tabsController = _tabsControllerFactory.Create(AreaMaterial, orderedAreaMaterials);
				_tabsController.StatusTabActive += OnStatusTabActive;
				
				if (!await _tabsController.PreloadAsync(content))
				{
					cancellationToken.ThrowIfCancellationRequested();
					
					_tabsController.StatusTabActive -= OnStatusTabActive;
					_tabsController.Unload();
					_tabsController = null;
					
					return false;
				}
                
				foreach (var material in orderedMaterials)
				{
					if ((_presenter.ContainsTabInHeader(material) || _presenter.AddTabToHeader(material))
					    && statusContent.ActiveMaterialId == material.Id)
					{
						SetStatusTabActive(statusContent, material);
					}
				}

				_presenter.ReorderTabsInHeader(orderedMaterials);
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
		
		private async UniTask<bool> AttemptToCreateTabAsync(ContentAreaMaterialData areaMaterial)
		{
			var materialId = areaMaterial.MaterialId;
			
			if (materialId == null)
			{
				Debug.LogError($"Material id is missing when trying to create a tab for area material: {areaMaterial}");
				return false;
			}
			
			try
			{
				var cancellationToken = _unloadCancellationTokenSource.Token;
				var material = await _helper.FindWindowMaterialAsync(materialId.Value, cancellationToken);
				
				if (material == null)
					return false;
				
				if (!await _tabsController.AddChildAsync(areaMaterial))
					return false;
				
				if (!_presenter.ContainsTabInHeader(material))
					_presenter.AddTabToHeader(material);
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
				return false;
			}

			return true;
		}

		private bool AttemptToRemoveTab(ContentAreaMaterialData areaMaterial)
		{
			var materialId = areaMaterial.MaterialId;

			if (materialId == null)
			{
				Debug.LogError($"Material id is missing when trying to remove a tab for area material: {areaMaterial}");
				return false;
			}
			
			var contentOrder = AreaMaterial.StatusContent.ContentOrder;
			
			if (contentOrder.Remove(areaMaterial.Id))
				areaMaterial.Order = null;
			
			if (!_tabsController.RemoveChild(areaMaterial) || !_presenter.RemoveTabFromHeader(materialId.Value))
				return false;
			
			AttemptToReorderTabsAsync(contentOrder, _unloadCancellationTokenSource.Token).Forget();
			return true;
		}

		private async UniTaskVoid HandleDataItemsAsync(IReadOnlyList<StatusContentItemData> dataItems)
		{
			var cancellationToken = _unloadCancellationTokenSource.Token;
			var contentOrder = AreaMaterial.StatusContent.ContentOrder;
			
			if (_presenter.TabsCount != contentOrder.Count + dataItems.Count)
			{
				try
				{
					await UniTask.WaitUntil(() => _presenter.TabsCount == contentOrder.Count + dataItems.Count, cancellationToken: cancellationToken);
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
			
			var areaMaterialIdsByOrder = new SortedDictionary<int, ulong>();
			
			for (var i = 0; i < dataItems.Count; i++)
			{
				var dataItem = dataItems[i];
				var areaMaterialId = dataItem.AreaId;
				var index = contentOrder.IndexOf(areaMaterialId);
				
				if (index >= 0)
					contentOrder.RemoveAt(index);
				
				areaMaterialIdsByOrder.Add(dataItem.Order, areaMaterialId);
			}
			
			if (areaMaterialIdsByOrder.Count > 0)
			{
				contentOrder.AddRange(areaMaterialIdsByOrder.Values);
				AttemptToReorderTabsAsync(contentOrder, cancellationToken).Forget();
			}
		}
		
		private void SetStatusTabActive(StatusContentMaterialData statusContent, WindowMaterialData material)
		{
			var materialId = material.Id;
			
			if (statusContent.ActiveMaterialId != materialId)
				statusContent.ActiveMaterialId = materialId;
			
			_presenter.ActivateTabInHeader(material);
		}
		
		private bool AttemptToPutTabIntoBuffer(ContentAreaMaterialData areaMaterial)
		{
			var materialId = areaMaterial.MaterialId;
			
			if (materialId == null)
			{
				Debug.LogError($"Material id is missing when trying to put a tab into the buffer for area material: {areaMaterial}");
				return false;
			}
			
			var contentOrder = AreaMaterial.StatusContent.ContentOrder;
			
			if (contentOrder.Remove(areaMaterial.Id))
				areaMaterial.Order = null;
			
			if (!_tabsController.RemoveChild(areaMaterial, out var child) || !_presenter.RemoveTabFromHeader(materialId.Value))
				return false;
			
			_dragAndDropBuffer.PutInto(child);
			return true;
		}
		
		private bool AttemptToExtractTabFromBuffer(ContentAreaMaterialData areaMaterial)
		{
			if (!_dragAndDropBuffer.Peek(out var child))
			{
				Debug.LogError("Child is missing in exchange buffer when trying to peek it");
				return false;
			}
			
			if (areaMaterial != child.AreaMaterial)
			{
				Debug.LogError($"The area material {areaMaterial} does not match the area material {child.AreaMaterial} of the child contained in the buffer");
				return false;
			}
			
			var areaMaterialId = areaMaterial.Id;
			var materialId = areaMaterial.MaterialId;
			
			if (materialId == null)
			{
				Debug.LogError($"Material id is missing when trying to extract a tab from the buffer for area material: {areaMaterial}");
				return false;
			}
			
			var contentOrder = AreaMaterial.StatusContent.ContentOrder;

			if (!contentOrder.Contains(areaMaterialId))
			{
				contentOrder.Add(areaMaterialId);
				areaMaterial.Order = contentOrder.Count;
			}
			
			var childrenCountBefore = _tabsController.ChildrenCount;
			
			if (!_tabsController.AddChild(child))
				return false;
			
			if (!_presenter.AddTabToHeader(_helper.GetMaterial<WindowMaterialData>(materialId.Value)))
				return false;
			
			_dragAndDropBuffer.Clear();
			
			if (childrenCountBefore == 0)
				_tabsController.SetChildActive(areaMaterial.Id);
			
			return true;
		}
		
		private async UniTask AttemptToReorderTabsAsync(IList<ulong> contentOrder, CancellationToken cancellationToken)
		{
			try
			{
				if (_isTabsReorderingInProgress)
					await UniTask.WaitWhile(() => _isTabsReorderingInProgress, cancellationToken: cancellationToken);
				
				_isTabsReorderingInProgress = true;
				
				var orderedMaterials = new List<WindowMaterialData>();
				
				if (await _helper.TryFindOrderedMaterialsAsync(contentOrder, orderedMaterials, cancellationToken))
					_presenter.ReorderTabsInHeader(orderedMaterials);
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				_isTabsReorderingInProgress = false;
			}
		}
		
		private void PerformAction(string actionAlias, ulong materialId)
		{
			if (_presenter == null)
			{
				Debug.LogError($"StatusColumnPresenter is not yet available when trying to perform action {actionAlias} with material id {materialId}");
				return;
			}
			
			if (actionAlias == WindowMaterialActionAlias.Focus)
			{
				if (!_presenter.InFocus)
					_presenter.Focus();
				else
					_presenter.Unfocus();
			}
			else
			{
				if (_tabsController != null)
					_tabsController.PerformAction(actionAlias, materialId);
				else 
					Debug.LogError($"StatusTabsController is null when trying to perform action {actionAlias} with material id {materialId}");
			}
		}
		
		private void OnWindowMaterialAction(IMessage message)
		{
			var action = (WindowMaterialAction) message.Data;
			var statusContentId = action.StatusContentId;
			
			if (statusContentId == null || AreaMaterial.StatusContent.Id != statusContentId.Value)
				return;
			
			PerformAction(action.Alias, action.MaterialId);
		}
		
		private void OnStatusTabActive(ContentAreaMaterialData areaMaterial)
		{
			var statusContent = AreaMaterial.StatusContent;
			var column = statusContent.Column;
			
			if (_options.IsSumAdaptiveModeActive && _options.SumAdaptiveModeColumn != column)
				return;
			
			if (_presenter == null)
			{
				Debug.LogError($"StatusColumnPresenter is not yet available when trying to handle the status activation on a column with area material: {AreaMaterial}");
				return;
			}
            
			if (_tabsController == null)
			{
				Debug.LogError($"StatusTabsController is null when trying to handle the status activation with window on a column with area material: {AreaMaterial}");
				return;
			}
			
			var materialId = areaMaterial.MaterialId;
			
			if (materialId == null)
			{
				Debug.LogError($"Material id is missing when trying to handle the status activation with window on a column with area material: {AreaMaterial}");
				return;
			}
			
			var material = _helper.GetMaterial<WindowMaterialData>(materialId.Value);
			
			if (_presenter.ContainsTabInHeader(material) || _presenter.AddTabToHeader(material))
				SetStatusTabActive(statusContent, material);
		}
		
		private void OnContentAreaCreated(IMessage message)
		{
			var areaMaterial = (ContentAreaMaterialData) message.Data;
			
			if (areaMaterial.ParentId != AreaMaterial.Id)
				return;
			
			if (_presenter == null)
			{
				Debug.LogError($"StatusColumnPresenter is not yet available when trying to handle the content area creation with content area material: {areaMaterial}");
				return;
			}
			
			if (_tabsController == null)
			{
				Debug.LogError($"StatusTabsController is null when trying to handle the content area creation with content area material: {areaMaterial}");
				return;
			}
			
			AttemptToCreateTabAsync(areaMaterial).Forget();
		}
		
		private void OnContentAreaRemoved(IMessage message)
		{
			var areaMaterial = (ContentAreaMaterialData) message.Data;
			
			if (areaMaterial.ParentId != AreaMaterial.Id)
				return;
			
			if (_presenter == null)
			{
				Debug.LogError($"StatusColumnPresenter is not yet available when trying to handle the content area removal with content area material: {areaMaterial}");
				return;
			}
			
			if (_tabsController == null)
			{
				Debug.LogError($"StatusTabsController is null when trying to handle the content area removal with content area material: {areaMaterial}");
				return;
			}

			AttemptToRemoveTab(areaMaterial);
		}
        
		private void OnStatusOrderUpdated(IMessage message)
		{
			var statusContent = AreaMaterial.StatusContent;
			var column = statusContent.Column;
			
			if (_options.IsSumAdaptiveModeActive && _options.SumAdaptiveModeColumn != column)
				return;
			
			if (_presenter == null)
			{
				Debug.LogError($"StatusColumnPresenter is not yet available when trying to handle a status order updating on a column with area material: {AreaMaterial}");
				return;
			}
			
			if (_tabsController == null)
			{
				Debug.LogError($"StatusTabsController is null when trying to handle a status order updating on a column with area material: {AreaMaterial}");
				return;
			}
			
			var action = (StatusContentUpdate) message.Sender;
			
			if (statusContent.Id != action.ContentId || column != action.Column) 
				return;
			
			AttemptToReorderTabsAsync(statusContent.ContentOrder, _unloadCancellationTokenSource.Token).Forget();
		}
		
		private void OnStatusContentAreaCreateMany(IMessage message)
		{
			var statusContent = AreaMaterial.StatusContent;
			var column = statusContent.Column;
			
			if (_options.IsSumAdaptiveModeActive && _options.SumAdaptiveModeColumn != column)
				return;
			
			if (_presenter == null)
			{
				Debug.LogError($"StatusColumnPresenter is not yet available when trying to handle a status content area creating on a column with area material: {AreaMaterial}");
				return;
			}
			
			if (_tabsController == null)
			{
				Debug.LogError($"StatusTabsController is null when trying to handle a status content area creating on a column with area material: {AreaMaterial}");
				return;
			}
			
			var action = (StatusContentAreaCreateMany) message.Sender;
			var dataItems = action.DataItems;
			
			for (var i = 0; i < dataItems.Count; i++)
			{
				var dataItem = dataItems[i];
				
				if (statusContent.Id != dataItem.ContentId || column != dataItem.Column)
					return;
			}
			
			HandleDataItemsAsync(dataItems).Forget();
		}
		
		private void OnContentAreaUpdated(IMessage message)
		{
			var areaMaterial = (ContentAreaMaterialData) message.Data;
			
			if (areaMaterial.ParentId == AreaMaterial.Id) // Do not remove or change this, otherwise a bug will be caught
				return;
			
			if (_presenter == null)
			{
				Debug.LogError($"StatusColumnPresenter is not yet available when trying to handle the content area updating with content area material: {areaMaterial}");
				return;
			}
			
			if (_tabsController == null)
			{
				Debug.LogError($"StatusTabsController is null when trying to handle the content area updating with content area material: {areaMaterial}");
				return;
			}
			
			if (_tabsController.ContainsChild(areaMaterial) && AttemptToPutTabIntoBuffer(areaMaterial))
				MessageDispatcher.SendMessage(this, MessageType.StatusTabItemDragAndDrop, areaMaterial, EnumMessageDelay.IMMEDIATE);
		}
		
		private void OnStatusTabItemDraggedAndDropped(IMessage message)
		{
			if (message.Sender == this)
				return;
			
			var areaMaterial = (ContentAreaMaterialData) message.Data;
			
			if (areaMaterial.ParentId != AreaMaterial.Id)
				return;
			
			if (_presenter == null)
			{
				Debug.LogError($"StatusColumnPresenter is not yet available when trying to handle the tab item dragged and dropped with content area material: {areaMaterial}");
				return;
			}
			
			if (_tabsController == null)
			{
				Debug.LogError($"StatusTabsController is null when trying to handle the tab item dragged and dropped with content area material: {areaMaterial}");
				return;
			}
			
			if (!_tabsController.ContainsChild(areaMaterial))
				AttemptToExtractTabFromBuffer(areaMaterial);
		}
	}
}