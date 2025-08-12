using System;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.Status.Data;
using Core.Elements.StatusColumn.Data;
using Core.Materials.Storage;
using Core.Messages;
using Core.Network.Api.Consts;
using Core.Network.Http;
using Core.Network.Http.Data;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Options;
using Core.User.Installation;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.Status.Presenter;
using Elements.StatusColumn.Controller;
using Elements.StatusColumn.Controller.Factory;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Elements.Status.Controller
{
	public class StatusController : IStatusController
	{
		private readonly int _maxColumns;
		private readonly IApplicationOptions _options;
		private readonly IMaterialDataStorage _materials;
		private readonly IDictionary<ulong, IStatusColumnController> _children;
		private readonly IStatusColumnControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IHttpRequest _request;
		private readonly IOutgoingStateController _outgoingState;
		private readonly CancellationTokenSource _unloadCancellationTokenSource;
		
		private IStatusPresenter _presenter;
		
		public StatusMaterialData Material { get; }
		public StatusAreaMaterialData AreaMaterial { get; }
		
		public StatusController(StatusMaterialData material,
			StatusAreaMaterialData areaMaterial,
			IUserInstallation installation,
			IApplicationOptions options,
			IMaterialDataStorage materials,
			IStatusColumnControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory,
			IHttpRequest request,
			IOutgoingStateController outgoingState)
		{
			Material = material;
			AreaMaterial = areaMaterial;
			
			_maxColumns = installation.StatusColumnCount ?? material.ColumnCount;
			_options = options;
			_materials = materials;
			_children = new Dictionary<ulong, IStatusColumnController>(_maxColumns);
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_request = request;
			_outgoingState = outgoingState;
			_unloadCancellationTokenSource = new CancellationTokenSource();
		}
		
		public async UniTask<bool> PreloadAsync(RectTransform parent)
		{
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return false;
			
			_presenter = _presenterFactory.Create<StatusPresenter>(parent);
			
			if (_presenter == null)
			{
				Debug.LogError($"Failed to instantiate the StatusPresenter by material {Material}");
				return false;
			}
			
			if (!_presenter.SetMaterial(Material, AreaMaterial))
				return false;
			
			_presenter.AlignToParent();
			
			if (!await PreloadChildrenAsync())
				return false;
			
			if (!_options.IsStateSendingAllowed)
			{
				_request.Request(RestApiUrl.LoadedStatuses,
					HttpMethodType.Patch,
					new [] { ("statusLoaded", (object) _outgoingState.Context.GetLoadedStatuses()) });
			}
			
			_outgoingState.Context.SetStatusAsPreloaded(Material.Id);
			_outgoingState.PrepareToSendIfAllowed();
			
			return true;
		}
		
		public bool Show()
		{
			if (_presenter == null || !_presenter.Show())
				return false;
			
			foreach (var child in _children.Values)
				child.Show();
			
			if (!_options.IsStateSendingAllowed)
				_request.Request(string.Format(RestApiUrl.ActiveStatusFormat, Material.Id), HttpMethodType.Patch);
			
			_outgoingState.Context.GetStatus().active_status_id = Material.Id;
			_outgoingState.PrepareToSendIfAllowed();
			
			MessageDispatcher.AddListener(MessageType.PreviewScreenModeMakeReady, OnPreviewMakeReady);
			
			return true;
		}
		
		public void ConfirmPlaylists(IList<int> columns)
		{
			var columnsCount = columns.Count;
			
			foreach (var child in _children.Values)
			{
				var column = child.AreaMaterial.StatusContent.Column;
				var columnIndex = column - 1;
				var isPlaylistEnabled = false;
				
				for (var i = 0; i < columnsCount; i++)
				{
					if (_options.IsSumAdaptiveModeActive && _options.SumAdaptiveModeColumn != column)
						return;
					
					if (columns[i] != columnIndex)
						continue;
					
					child.EnablePlaylist();
					isPlaylistEnabled = true;
					break;
				}
				
				if (!isPlaylistEnabled)
					child.AttemptToDisablePlaylist();
			}
		}
		
		public bool Hide()
		{
			MessageDispatcher.RemoveListener(MessageType.PreviewScreenModeMakeReady, OnPreviewMakeReady);
			
			foreach (var child in _children.Values)
				child.Hide();
			
			return _presenter != null && _presenter.Hide();
		}

		public void Unload()
		{
			MessageDispatcher.RemoveListener(MessageType.PreviewScreenModeMakeReady, OnPreviewMakeReady);
			
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

			_outgoingState.Context.SetStatusAsUnloaded(Material.Id);
			_outgoingState.ForceToSendIfAllowed();
		}
		
		private IList<StatusContentAreaMaterialData> FindAreaMaterials(ulong materialId)
		{
			var areaMaterials = new List<StatusContentAreaMaterialData>();
					
			foreach (var areaMaterial in _materials.GetList<StatusContentAreaMaterialData>())
			{
				var statusContent = areaMaterial?.StatusContent;
						
				if (statusContent == null)
					continue;
						
				if (_options.IsSumAdaptiveModeActive && _options.SumAdaptiveModeColumn != statusContent.Column)
					continue;
						
				if (statusContent.Column > _maxColumns)
					continue;
						
				if (statusContent.StatusId == materialId)
					areaMaterials.Add(areaMaterial);
			}
					
			return areaMaterials;
		}
		
		private async UniTask<bool> PreloadChildrenAsync()
		{
			try
			{
				var cancellationToken = _unloadCancellationTokenSource.Token;
				
				cancellationToken.ThrowIfCancellationRequested();
				
				var areaMaterials = FindAreaMaterials(Material.Id);
				
				if (areaMaterials == null)
					return false;
				
				foreach (var areaMaterial in areaMaterials)
				{
					await UniTask.NextFrame(cancellationToken);
					
					if (!await PreloadChildAsync(areaMaterial, cancellationToken))
						Debug.LogError($"Attempt to preload status column with area material id {areaMaterial.Id} is failed");
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

		private async UniTask<bool> PreloadChildAsync(StatusContentAreaMaterialData areaMaterial, CancellationToken cancellationToken)
		{
			var materialId = areaMaterial.StatusContent.Id;
			
			if (_children.ContainsKey(materialId))
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
			
			if (!_children.TryAdd(materialId, child))
			{
				child.Unload();
				return false;
			}
			
			return true;
		}
		
		private void OnPreviewMakeReady(IMessage message)
		{
			if (_presenter is { Visible: true })
				MessageDispatcher.SendMessageData(MessageType.PreviewScreenLayersMakeReady, AreaMaterial);
		}
	}
}