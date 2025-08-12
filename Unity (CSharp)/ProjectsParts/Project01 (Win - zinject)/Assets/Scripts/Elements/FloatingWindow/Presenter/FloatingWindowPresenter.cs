using System;
using Base.Presenter;
using Core.Elements.Windows.Base.Data;
using Core.Network.Socket.Packets.Incoming.Actions.Consts;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Options;
using Cysharp.Threading.Tasks;
using Elements.FloatingWindow.Presenter.Consts;
using Elements.FloatingWindow.Presenter.Enums;
using Elements.FloatingWindow.Presenter.WindowAdapters.Base;
using Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components;
using UI.Canvas.Presenter;
using UI.ShadedElements.Base;
using UI.ShadedElements.Controller;
using UI.ShadedElements.Presenter.Targets.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Debug = Core.Logging.Debug;

namespace Elements.FloatingWindow.Presenter
{
	public class FloatingWindowPresenter : PresenterBase, IFloatingWindowPresenter, IFocusCapable,
		IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private ResizingArea _resizingArea;
		[SerializeField] private Vector2 _minSize;
		[SerializeField] private Vector2 _defaultCustomSize;
		[SerializeField] private Vector2 _screenCornerSize;
		[SerializeField] private Vector2 _oneThirdScreenSize;
		
		private ICanvasPresenter _root;
		private IInFocusCollection _inFocusCollection;
		private IOutgoingStateController _outgoingState;
		
		private Camera _mainCamera;
		private IWindowPresenterAdapter _windowAdapter;
		
		private Vector2 _bottomLeftParentCorner;
		private Vector2 _topRightParentCorner;
		
		private Vector2 _worldSize;
		
		private string _uuid;
		private string _tagOpen;
		private string _sourceOpen;
		
		private bool _isAdaptiveSizeRequired;
		private bool _isAspectRatioEnabled;
		private bool _isResetActionReady;
		
		private string _previousScreenPosition;
		private string _currentScreenPosition;
		
		private Vector3 _originPosition;
		private Vector2 _originPivot;
		private Vector2 _originSizeDelta;
		private Vector2 _originAspectRatioSizeDelta;
		
		private Vector2 _sizeDelta;
		private Vector2 _anchoredPosition;
		
		private bool _isMouseOver;
		private bool _isResizing;
		private bool _isDragging;
		
		public bool Active { get; private set; }
		public bool InFocus => _inFocusCollection.Contains(this);
		public RectTransform OriginalParent { get; private set; }
		
		[Inject]
		private void Initialize(IApplicationOptions options,
			ICanvasPresenter root,
			IShadedFloatingWindowsController inFocusCollection,
			IOutgoingStateController outgoingState)
		{
			_root = root;
			_inFocusCollection = inFocusCollection;
			_outgoingState = outgoingState;
			_mainCamera = Camera.main;
			_uuid = Guid.NewGuid().ToString();
			
			ResetAllDefaultSizes(options);
			
			_isAspectRatioEnabled = true;
		}
		
		public override void SetParentOnInitialize(RectTransform parent)
		{
			OriginalParent = parent;
			
			base.SetParentOnInitialize(parent);
			
			var parentRect = parent.rect;
			var localToWorldParentMatrix = parent.transform.localToWorldMatrix;
			
			_bottomLeftParentCorner = localToWorldParentMatrix.MultiplyPoint(new Vector3(parentRect.x, parentRect.y));
			_topRightParentCorner = localToWorldParentMatrix.MultiplyPoint(new Vector3(parentRect.xMax, parentRect.yMax));
			
			_worldSize = CalculateWorldSize();
		}
		
		public void AllowAdaptiveSize() => _isAdaptiveSizeRequired = true;
		
		public bool SetWindowAdapter(IWindowPresenterAdapter windowAdapter)
		{
			if (windowAdapter == null)
			{
				Debug.LogError("An attempt was detected to assign null window adapter to the FloatingWindowPresenter");
				return false;
			}
			
			if (_windowAdapter != null)
			{
				Debug.LogError("The window adapter has already been assigned before. Not allowed to reassign window adapter to the FloatingWindowPresenter");
				return false;
			}
			
			_windowAdapter = windowAdapter;
			
			SetName($"FloatingWindow: {_uuid}");
			return true;
		}
		
		public void SetOptions(string tagOpen, string sourceOpen)
		{
			_tagOpen = tagOpen;
			_sourceOpen = sourceOpen;
		}
		
		public async UniTask<bool> PreloadAsync()
		{
			var rectTransform = RectTransform;
			var parentSize = Parent.rect.size;
			
			rectTransform.anchorMin = rectTransform.anchorMax = Vector2.one * 0.5f;
			
			rectTransform.sizeDelta = new Vector2(Mathf.Clamp(_defaultCustomSize.x, _minSize.x, parentSize.x),
				Mathf.Clamp(_defaultCustomSize.y, _minSize.y, parentSize.y));
			
			_originAspectRatioSizeDelta = rectTransform.sizeDelta;
			
			_worldSize = CalculateWorldSize();
			
			return await _windowAdapter.PreloadAsync();
		}
		
		public bool SetState(WindowState state) => _windowAdapter.SetState(state);
		
		public void Activate() => Active = true;
		public void Deactivate() => Active = false;
		
		public override bool Show()
		{
			if (!base.Show())
				return false;
			
			var rectTransform = RectTransform;
			
			_sizeDelta = rectTransform.sizeDelta;
			_anchoredPosition = rectTransform.anchoredPosition;
			
			_windowAdapter.Show();
			
			if (_isAdaptiveSizeRequired)
			{
				_windowAdapter.ContentDisplayed += OnWindowContentDisplayed;
				
				if (_windowAdapter.IsContentDisplayed)
					AdaptSizeFor(rectTransform, Parent.rect.size);
			}
			
			EnableComponents();
			PrepareToSendState();
			return true;
		}
		
		public void Focus()
		{
			if (_inFocusCollection.Add(this))
				PrepareToSendState();
		}

		public void Unfocus()
		{
			if (_inFocusCollection.Remove(this))
				PrepareToSendState();
		}

		public void PerformAction(string actionAlias, ulong materialId)
		{
			if (!Visible) 
				return;
			
			_windowAdapter.PerformAction(actionAlias, materialId);
			
			if (actionAlias == WindowMaterialActionAlias.FullscreenToggle)
				PrepareToSendState();
		}
		
		public void PerformFloatingWindowAction(string action)
		{
			if (!Visible)
				return;
			
			switch (action)
			{
				case FloatingWindowVideoStreamMaterialActionAlias.AspectRation:
				{
					ToggleAspectRatio();
					break;
				}
				case FloatingWindowVideoStreamMaterialActionAlias.PositionReset:
				{
					if (_previousScreenPosition != null)
						SetScreenPosition(_previousScreenPosition);
					
					break;
				}
				default:
				{
					SetScreenPosition(ScreenPositionType.GetPosition(action));
					break;
				}
			}
			
			PrepareToSendState();
		}
		
		public override bool Hide()
		{
			if (!base.Hide())
				return false;
			
			DisableComponents();
			
			if (_isAdaptiveSizeRequired)
				_windowAdapter.ContentDisplayed -= OnWindowContentDisplayed;
			
			_windowAdapter.Hide();
			
			SetDefaults();
			RemoveState();
			return true;
		}
		
		public override void Unload()
		{
			Active = false;
			
			DisableComponents();
			
			if (_isAdaptiveSizeRequired)
				_windowAdapter.ContentDisplayed -= OnWindowContentDisplayed;
			
			if (_windowAdapter != null)
			{
				_windowAdapter.Unload();
				_windowAdapter = null;
			}
			
			SetDefaults();

			_isResetActionReady = false;
			
			_previousScreenPosition = null;
			_currentScreenPosition = null;
			
			_mainCamera = null;
			
			_outgoingState = null;
			_inFocusCollection = null;
			_root = null;
			
			OriginalParent = null;
			
			_uuid = null;
			_tagOpen = null;
			_sourceOpen = null;
			
			_bottomLeftParentCorner = default;
			_topRightParentCorner = default;

			_worldSize = default;
			
			base.Unload();
		}
		
		private void UpdateContent()
		{
			_windowAdapter.UpdateContent();
			_worldSize = CalculateWorldSize();
		}
		
		private void AdaptSizeFor(RectTransform rectTransform, Vector2 parentSize)
		{
			var originalContentSize = _windowAdapter.OriginalContentSize;
			
			var sizeDelta = new Vector2();
			
			if (originalContentSize.x < _minSize.x)
			{
				sizeDelta.x = _minSize.x;
				sizeDelta.y = originalContentSize.y * originalContentSize.x / _minSize.x;
			}
			else if (originalContentSize.x > parentSize.x)
			{
				sizeDelta.x = parentSize.x;
				sizeDelta.y = originalContentSize.y * originalContentSize.x / parentSize.x;
			}
			else
			{
				sizeDelta.x = originalContentSize.x;
			}
			
			if (originalContentSize.y < _minSize.y)
			{
				sizeDelta.y = _minSize.y;
				sizeDelta.x = originalContentSize.x * originalContentSize.y / _minSize.y;
			}
			else if (originalContentSize.y > parentSize.y)
			{
				sizeDelta.y = parentSize.y;
				sizeDelta.x = originalContentSize.x * originalContentSize.y / parentSize.y;
			}
			else
			{
				sizeDelta.y = originalContentSize.y;
			}
			
			rectTransform.sizeDelta = sizeDelta;
			
			UpdateContent();
			
			_originAspectRatioSizeDelta = rectTransform.sizeDelta;
		}
		
		private float ValidateValue(float? optionalValue, float value) => 
			optionalValue is > 0 ? optionalValue.Value : value;
		
		private void ResetAllDefaultSizes(IApplicationOptions options)
		{
			_minSize = new Vector2(ValidateValue(options.MinFloatingWindowSizeX, _minSize.x),
				ValidateValue(options.MinFloatingWindowSizeY, _minSize.y));
			
			_defaultCustomSize = new Vector2(ValidateValue(options.DefaultCustomFloatingWindowSizeX, _defaultCustomSize.x),
				ValidateValue(options.DefaultCustomFloatingWindowSizeY, _defaultCustomSize.y));
			
			_screenCornerSize = new Vector2(ValidateValue(options.ScreenCornerFloatingWindowSizeX, _screenCornerSize.x), 
				ValidateValue(options.ScreenCornerFloatingWindowSizeY, _screenCornerSize.y));
			
			_oneThirdScreenSize = new Vector2(ValidateValue(options.OneThirdScreenFloatingWindowSizeX, _oneThirdScreenSize.x), 
				ValidateValue(options.OneThirdScreenFloatingWindowSizeY, _oneThirdScreenSize.y));
		}
		
		private void SetDefaults()
		{
			_sizeDelta = default;
			_anchoredPosition = default;
			
			_originPosition = default;
			_originPivot = default;
			_originSizeDelta = default;
			
			_isMouseOver = false;
			_isResizing = false;
			_isDragging = false;
		}
		
		private Vector2 CalculateWorldSize()
		{
			var rectTransform = RectTransform;
			var rect = rectTransform.rect;
			var localToWorldMatrix = rectTransform.localToWorldMatrix;
			
			var bottomLeftCorner = localToWorldMatrix.MultiplyPoint(new Vector3(rect.x, rect.y));
			var topRightCorner = localToWorldMatrix.MultiplyPoint(new Vector3(rect.xMax, rect.yMax));
			
			return new Vector2(Mathf.Abs(topRightCorner.x - bottomLeftCorner.x), Mathf.Abs(topRightCorner.y - bottomLeftCorner.y));
		}
		
		private void Update()
		{
			if (_isMouseOver && Input.GetMouseButtonDown(0))
			{
				var rectTransform = RectTransform;
				var screenPoint = (Vector2) Input.mousePosition;
				
				if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, _mainCamera, out var localPoint))
					return;
				
				if (rectTransform.rect.Contains(localPoint))
					rectTransform.SetAsLastSibling();
			}
		}
		
		private void PrepareToSendState()
		{
			var material = _windowAdapter.Material;
			
			_outgoingState.Context.SetWindowState(_uuid, material, Visible, _windowAdapter.IsInFullScreenMode, InFocus, _tagOpen, _sourceOpen);
			
			switch (_tagOpen)
			{
				case TagOpen.Video:
					_windowAdapter.SetVideoStateInOutgoingState(_uuid);
					break;
				case TagOpen.SidebarVks or TagOpen.SidebarHdmi:
					_outgoingState.Context.SetFloatingVideoStreamWindowState(material, _currentScreenPosition, _isAspectRatioEnabled, _isResetActionReady);
					break;
			}
			
			_outgoingState.PrepareToSendIfAllowed();
		}
		
		private void RemoveState()
		{
			_outgoingState.Context.RemoveFloatingWindowState(_uuid);
			
			switch (_tagOpen)
			{
				case TagOpen.Video:
					_windowAdapter.RemoveVideoStateFromOutgoingState();
					break;
				case TagOpen.SidebarVks or TagOpen.SidebarHdmi:
					_outgoingState.Context.RemoveFloatingVideoStreamWindowState(_windowAdapter.Material);
					break;
			}
			
			_outgoingState.PrepareToSendIfAllowed();
		}
		
		private void EnableComponents()
		{
			_resizingArea.ResizeStarted += OnResizeStarted;
			_resizingArea.Resize += OnResize;
			_resizingArea.ResizeStopped += OnResizeStopped;

			if (_windowAdapter != null)
			{
				_windowAdapter.StateChanged += OnStateChanged;
				_windowAdapter.DragStarted += OnDragStarted;
				_windowAdapter.Drag += OnDrag;
				_windowAdapter.DragStopped += OnDragStopped;
				_windowAdapter.Closed += OnClosed;
			}
		}
		
		private void DisableComponents()
		{
			if (_windowAdapter != null)
			{
				_windowAdapter.StateChanged -= OnStateChanged;
				_windowAdapter.DragStarted -= OnDragStarted;
				_windowAdapter.Drag -= OnDrag;
				_windowAdapter.DragStopped -= OnDragStopped;
				_windowAdapter.Closed -= OnClosed;
			}
			
			_resizingArea.ResizeStarted -= OnResizeStarted;
			_resizingArea.Resize -= OnResize;
			_resizingArea.ResizeStopped -= OnResizeStopped;
		}
		
		private void ToggleAspectRatio()
		{
			_isAspectRatioEnabled = !_isAspectRatioEnabled;
			_originAspectRatioSizeDelta = RectTransform.sizeDelta;
		}
		
		private void SetScreenPosition(string screenPosition)
		{
			_previousScreenPosition = null;
			_currentScreenPosition = screenPosition;
			_isResetActionReady = false;

			var oneThirdScreenSizeX = _oneThirdScreenSize.x;
			var oneThirdScreenSizeY = _oneThirdScreenSize.y;
			
			var screenCornerSizeX = _screenCornerSize.x;
			var screenCornerSizeY = _screenCornerSize.y;
			
			var parentRect = Parent.rect;
			var parentWidth = parentRect.width;
			var parentHeight = parentRect.height;
			
			var rectTransform = RectTransform;
			
			rectTransform.sizeDelta = screenPosition switch
			{
				ScreenPositionType.LeftPosition or ScreenPositionType.RightPosition or ScreenPositionType.CenterPosition => 
					new Vector2(Mathf.Min(parentWidth, oneThirdScreenSizeX), Mathf.Min(parentHeight, oneThirdScreenSizeY)),
				ScreenPositionType.AngleLeftPosition or ScreenPositionType.AngleRightPosition => 
					new Vector2(Mathf.Min(parentWidth, screenCornerSizeX), Mathf.Min(parentHeight, screenCornerSizeY)),
				_ => _sizeDelta
			};
			
			var rect = rectTransform.rect;
			var width = rect.width;
			var height = rect.height;
			
			rectTransform.anchoredPosition = screenPosition switch
			{
				ScreenPositionType.LeftPosition => new Vector2((width - parentWidth) * 0.5f + Mathf.Max((parentWidth / 3 - oneThirdScreenSizeX) * 0.5f, 0f), 0f),
				ScreenPositionType.RightPosition => new Vector2((parentWidth - width) * 0.5f - Mathf.Max((parentWidth / 3 - oneThirdScreenSizeX) * 0.5f, 0f), 0f),
				ScreenPositionType.AngleLeftPosition => new Vector2((width - parentWidth) * 0.5f , (parentHeight - height) * 0.5f),
				ScreenPositionType.AngleRightPosition => new Vector2((parentWidth - width) * 0.5f, (parentHeight - height) * 0.5f),
				ScreenPositionType.CenterPosition => Vector2.zero,
				_ => _anchoredPosition
			};
			
			UpdateContent();
		}
		
		private void SetSizeDeltaWithAspectRatio(Vector2 sizeDelta, Vector2 maxSizeDelta)
		{
			var newSizeDelta = sizeDelta * 10000f;
			
			if (newSizeDelta.x > maxSizeDelta.x)
			{
				newSizeDelta.y = maxSizeDelta.x / newSizeDelta.x * newSizeDelta.y;
				newSizeDelta.x = maxSizeDelta.x;
			}
			
			if (newSizeDelta.y > maxSizeDelta.y)
			{
				newSizeDelta.x = maxSizeDelta.y / newSizeDelta.y * newSizeDelta.x;
				newSizeDelta.y = maxSizeDelta.y;
			}
			
			RectTransform.sizeDelta = newSizeDelta;
		}
		
		private void OnWindowContentDisplayed() => AdaptSizeFor(RectTransform, Parent.rect.size);
		
		public void OnPointerEnter(PointerEventData eventData) => _isMouseOver = true;
		public void OnPointerExit(PointerEventData eventData) => _isMouseOver = false;
		
		private void OnResizeStarted(WindowCorner corner)
		{
			if (corner == WindowCorner.None || _isResizing)
				return;
			
			_isDragging = false;
			_isResizing = true;
			
			_windowAdapter.Lock();
			
			var rectTransform = RectTransform;
			
			var mousePosition = Input.mousePosition;
			
			mousePosition = new Vector3(mousePosition.x, mousePosition.y, _root.CanvasPlaneDistance);
			
			var worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
			
			_originPosition = Parent.InverseTransformPoint(worldPosition);
			_originPivot = rectTransform.pivot;
			_originSizeDelta = rectTransform.sizeDelta;
			
			var rect = rectTransform.rect;
			var widthHalf = rect.width * 0.5f;
			var heightHalf = rect.height * 0.5f;
			
			rectTransform.anchoredPosition += corner switch
			{
				WindowCorner.TopLeft => new Vector2(widthHalf, -heightHalf),
				WindowCorner.BottomLeft => new Vector2(widthHalf, heightHalf),
				WindowCorner.TopRight => new Vector2(-widthHalf, -heightHalf),
				WindowCorner.BottomRight => new Vector2(-widthHalf, heightHalf),
				_ => default
			};
			
			rectTransform.pivot = corner switch
			{
				WindowCorner.TopLeft => new Vector2(1f, 0f),
				WindowCorner.BottomLeft => new Vector2(1f, 1f),
				WindowCorner.TopRight => new Vector2(0f, 0f),
				WindowCorner.BottomRight => new Vector2(0f, 1f),
				_ => default
			};
		}
		
		private void OnResize(WindowCorner corner)
		{
			if (corner == WindowCorner.None || !_isResizing)
				return;
			
			_isDragging = false;
			
			var mousePosition = Input.mousePosition;
			
			mousePosition = new Vector3(mousePosition.x, mousePosition.y, _root.CanvasPlaneDistance);
			
			var worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
			
			worldPosition.x = Mathf.Clamp(worldPosition.x, _bottomLeftParentCorner.x, _topRightParentCorner.x);
			worldPosition.y = Mathf.Clamp(worldPosition.y, _bottomLeftParentCorner.y, _topRightParentCorner.y);
			
			var delta = Parent.InverseTransformPoint(worldPosition) - _originPosition;
			
			var newSizeDelta = _originSizeDelta + corner switch
			{
				WindowCorner.TopLeft => new Vector2(-delta.x, delta.y),
				WindowCorner.BottomLeft => new Vector2(-delta.x, -delta.y),
				WindowCorner.TopRight => new Vector2(delta.x, delta.y),
				WindowCorner.BottomRight => new Vector2(delta.x, -delta.y),
				_ => default
			};
			
			if (newSizeDelta.x < _minSize.x)
				newSizeDelta.x = _minSize.x;
			
			if (newSizeDelta.y < _minSize.y)
				newSizeDelta.y = _minSize.y;
			
			if (_isAspectRatioEnabled && _originAspectRatioSizeDelta != Vector2.zero)
				SetSizeDeltaWithAspectRatio(_originAspectRatioSizeDelta, newSizeDelta);
			else
				RectTransform.sizeDelta = newSizeDelta;
			
			UpdateContent();
		}
		
		protected virtual void OnResizeStopped(WindowCorner corner)
		{
			if (corner == WindowCorner.None || !_isResizing)
				return;
			
			_isDragging = false;
			_isResizing = false;
			
			var rectTransform = RectTransform;
			
			var rect = rectTransform.rect;
			
			var widthHalf = rect.width * 0.5f;
			var heightHalf = rect.height * 0.5f;
			
			rectTransform.anchoredPosition -= corner switch
			{
				WindowCorner.TopLeft => new Vector2(widthHalf, -heightHalf),
				WindowCorner.BottomLeft => new Vector2(widthHalf, heightHalf),
				WindowCorner.TopRight => new Vector2(-widthHalf, -heightHalf),
				WindowCorner.BottomRight => new Vector2(-widthHalf, heightHalf),
				_ => default
			};
			
			rectTransform.pivot = _originPivot;
			
			if (_currentScreenPosition != null)
			{
				_sizeDelta = rectTransform.sizeDelta;
				_anchoredPosition = rectTransform.anchoredPosition;
				
				_previousScreenPosition = _currentScreenPosition;
				_currentScreenPosition = null;
				
				_isResetActionReady = true;
			}
			
			_originPosition = default;
			_originPivot = default;
			_originSizeDelta = default;
			
			_windowAdapter.Unlock();
			
			PrepareToSendState();
		}
		
		private void OnStateChanged() => PrepareToSendState();
		
		private void OnDragStarted()
		{
			if (_isDragging)
				return;

			_isResizing = false;
			_isDragging = true;
			
			_windowAdapter.Lock();
			
			var mousePosition = Input.mousePosition;
			
			mousePosition = new Vector3(mousePosition.x, mousePosition.y, _root.CanvasPlaneDistance);
			
			_originPosition = RectTransform.position - _mainCamera.ScreenToWorldPoint(mousePosition);
		}
		
		private void OnDrag()
		{
			if (!_isDragging)
				return;

			_isResizing = false;
			
			var mousePosition = Input.mousePosition;
			
			mousePosition = new Vector3(mousePosition.x, mousePosition.y, _root.CanvasPlaneDistance);
			
			var position = _mainCamera.ScreenToWorldPoint(mousePosition) + _originPosition;
			
			var worldWidthHalf = _worldSize.x * 0.5f;
			var worldHeightHalf = _worldSize.y * 0.5f;
			
			var x = Mathf.Clamp(position.x, 
				_bottomLeftParentCorner.x + worldWidthHalf, 
                _topRightParentCorner.x - worldWidthHalf);
			
			var y = Mathf.Clamp(position.y, 
				_bottomLeftParentCorner.y + worldHeightHalf, 
				_topRightParentCorner.y - worldHeightHalf);
			
			var z = Mathf.Clamp(position.z, position.z, position.z);
			
			RectTransform.position = new Vector3(x, y, z);
		}
		
		protected virtual void OnDragStopped()
		{
			if (_isResizing || !_isDragging)
				return;
			
			_isResizing = false;
			_isDragging = false;
			
			var rectTransform = RectTransform;
			
			if (_currentScreenPosition != null)
			{
				_sizeDelta = rectTransform.sizeDelta;
				_anchoredPosition = rectTransform.anchoredPosition;
				
				_previousScreenPosition = _currentScreenPosition;
				_currentScreenPosition = null;
				
				_isResetActionReady = true;
			}
			
			_originPosition = default;
			
			_windowAdapter.Unlock();
			
			PrepareToSendState();
		}

		private void OnClosed()
		{
			Deactivate();
			Hide();
		}
	}
}
