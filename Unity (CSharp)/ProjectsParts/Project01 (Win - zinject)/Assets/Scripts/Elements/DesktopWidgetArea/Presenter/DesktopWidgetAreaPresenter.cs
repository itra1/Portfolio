using Base.Presenter;
using com.ootii.Messages;
using Core.Materials.Data;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Settings;
using Elements.Widgets.Base.Presenter;
using UnityEngine;
using Zenject;

namespace Elements.DesktopWidgetArea.Presenter
{
	public class DesktopWidgetAreaPresenter : PresenterBase, IDesktopWidgetAreaPresenter
	{
		private IProjectSettings _projectSettings;
		private IOutgoingStateController _outgoingState;
		
		private ContentAreaMaterialData _areaMaterial;
		private IWidgetAreaUpdateHandler _child;
		
		public ulong? AreaId => _areaMaterial?.Id;
		
		[Inject]
		private void Initialize(IProjectSettings projectSettings, IOutgoingStateController outgoingState)
		{
			_projectSettings = projectSettings;
			_outgoingState = outgoingState;
		}

		public bool SetMaterial(ContentAreaMaterialData areaMaterial)
		{
			if (areaMaterial == null)
			{
				Debug.LogError("An attempt was detected to assign a null area material to the DesktopWidgetAreaPresenter");
				return false;
			}
			
			if (_areaMaterial != null)
			{
				Debug.LogError("The area material has already been assigned before. Not allowed to reassign area material to the DesktopWidgetAreaPresenter");
				return false;
			}
			
			_areaMaterial = areaMaterial;
			
			SetName($"Widget: {areaMaterial.Id} - {areaMaterial.Name}");
			
			MessageDispatcher.AddListener(_areaMaterial.UpdateMessageType, OnAreaMaterialDataUpdated);
			
			return true;
		}

		public override void AlignToParent()
		{
			base.AlignToParent();
			
			var rectTransform = RectTransform;
			var parentRect = Parent.rect;
			var parentWidth = parentRect.width;
			var parentHeight = parentRect.height;
			var gridSize = _projectSettings.GridSize;
			var gridSizeX = gridSize.x;
			var gridSizeY = gridSize.y;
			var column = _areaMaterial.Column;
			var row = _areaMaterial.Row;
			var columnSize = parentWidth / gridSizeX;
			var rowSize = parentHeight / gridSizeY;
			var sizeX = _areaMaterial.SizeX > 0 ? _areaMaterial.SizeX : gridSizeX;
			var sizeY = _areaMaterial.SizeY > 0 ? _areaMaterial.SizeY : gridSizeY;
			var sizeDeltaX = parentWidth * (sizeX / gridSizeX - 1);
			var sizeDeltaY = parentHeight * (sizeY / gridSizeY - 1);
			var anchoredPositionX = columnSize * (column - 1) + sizeDeltaX * 0.5f;
			var anchoredPositionY = -(rowSize * (row - 1) + sizeDeltaY * 0.5f);
			
			rectTransform.anchoredPosition = new Vector2(anchoredPositionX, anchoredPositionY);
			rectTransform.sizeDelta = new Vector2(sizeDeltaX, sizeDeltaY);
		}
		
		public void IdentifyOnlyChild()
		{
			var rectTransform = RectTransform;
			
			if (rectTransform.childCount == 0)
			{
				Debug.LogError("The child was not found when trying to identify it in the DesktopWidgetAreaPresenter");
				return;
			}
			
			var childTransform = rectTransform.GetChild(0);
			
			if (childTransform.TryGetComponent<IWidgetAreaUpdateHandler>(out var child))
			{
				_child = child;
				
				if (_areaMaterial.MaterialId != null)
				{
					_outgoingState.Context.AddWidget(_areaMaterial.MaterialId.Value, _areaMaterial.Id);
					_outgoingState.PrepareToSendIfAllowed();
				}
			}
		}
		
		public override void Unload()
		{
			if (_areaMaterial != null)
			{
				if (_areaMaterial.MaterialId != null)
				{
					_outgoingState.Context.RemoveWidget(_areaMaterial.MaterialId.Value, _areaMaterial.Id);
					_outgoingState.PrepareToSendIfAllowed();
				}
				
				MessageDispatcher.RemoveListener(_areaMaterial.UpdateMessageType, OnAreaMaterialDataUpdated);
				
				_areaMaterial = null;
			}

			_child = null;
			
			_projectSettings = null;
			_outgoingState = null;
			
			base.Unload();
		}
		
		private void OnAreaMaterialDataUpdated(IMessage message)
		{
			if (_areaMaterial == null || message.Sender is not ContentAreaMaterialData areaMaterial || areaMaterial.Id != _areaMaterial.Id)
				return;
			
			AlignToParent();

			if (_child == null)
			{
				Debug.LogWarning("A child is missing when trying to give it to handle an area material data update");
				return;
			}
			
			_child.HandleAreaMaterialDataUpdate();
		}
	}
}