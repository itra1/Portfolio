using System.Collections.Generic;
using System.Threading;
using Base.Presenter;
using com.ootii.Messages;
using Core.Elements.StatusColumn.Data;
using Core.Elements.Windows.Base.Data;
using Core.Messages;
using Core.Options;
using Core.Settings;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Elements.StatusColumn.Data.Utils;
using Elements.StatusColumn.Presenter.Components;
using Elements.StatusColumn.Presenter.Components.Factory;
using Elements.Windows.Base.Data.Utils;
using Preview;
using Settings;
using UI.ShadedElements.Base;
using UI.ShadedElements.Controller;
using UI.ShadedElements.Presenter.Targets.Base;
using UI.Switches;
using UI.Switches.Triggers.Data;
using UnityEngine;
using Zenject;

namespace Elements.StatusColumn.Presenter
{
	public class StatusColumnPresenter : PresenterBase, IStatusColumnPresenter, IFocusCapable
	{
		[SerializeField] private StatusHeader _header;
		[SerializeField] private TriggerKey _triggerKey;
		
		private IProjectSettings _projectSettings;
		private IApplicationOptions _options;
		private ITriggerSwitch _triggerSwitch;
		private IInFocusCollection _inFocusCollection;
		private CancellationTokenSource _unloadCancellationTokenSource;
		
		private StatusContentAreaMaterialData _areaMaterial;
		private bool _previewEnabled;
		
		public RectTransform OriginalParent { get; private set; }
		public bool InFocus => _inFocusCollection.Contains(this);
		
		public int TabsCount => _header.TabsCount;
		
		[Inject]
		private void Initialize(IProjectSettings projectSettings,
			IUISettings uiSettings,
			IApplicationOptions options,
			IPreviewState previewState,
			ITriggerSwitch triggerSwitch,
			IShadedScreenModesController inFocusCollection, 
			IStatusHeaderTabFactory headerTabFactory)
		{
			_projectSettings = projectSettings;
			_options = options;
			_triggerSwitch = triggerSwitch;
			_inFocusCollection = inFocusCollection;
			_unloadCancellationTokenSource = new CancellationTokenSource();
			_previewEnabled = previewState.Enabled;
			
			_header.Initialize(uiSettings, headerTabFactory);
		}
		
		public bool SetMaterial(StatusContentAreaMaterialData areaMaterial)
		{
			if (areaMaterial == null)
			{
				Debug.LogError("An attempt was detected to assign a null area material to the StatusColumnPresenter");
				return false;
			}
			
			if (_areaMaterial != null)
			{
				Debug.LogError("The area material has already been assigned before. Not allowed to reassign area material to the StatusColumnPresenter");
				return false;
			}
			
			_areaMaterial = areaMaterial;
			
			var material = areaMaterial.StatusContent;
			
			if (!_options.IsSumAdaptiveModeActive)
			{
				_triggerKey = material.GetTriggerKey();
				
				if (_triggerKey != default)
					_triggerSwitch.AddListener(_triggerKey, OnStatusColumnToggled);
			}
			
			SetName($"Column: [{areaMaterial.Id}, {material.Id}] - {material.Name}");
			
			return true;
		}
		
		public override void SetParentOnInitialize(RectTransform parent)
		{
			OriginalParent = parent; 
			base.SetParentOnInitialize(parent);
		}
		
		public override void AlignToParent()
		{
			base.AlignToParent();
			
			var rectTransform = RectTransform;
			
			if (_options.IsSumAdaptiveModeActive)
			{
				rectTransform.anchoredPosition = Vector2.zero;
				rectTransform.sizeDelta = Vector2.zero;
			}
			else
			{
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
		}

		public override bool Show()
		{
			if (!base.Show())
				return false;
			
			if (!_options.IsSumAdaptiveModeActive)
			{
				if (_triggerSwitch.IsOn(_triggerKey))
					ShowHeader();
				else
					HideHeader();
			}
			else
			{
				HideHeader();
			}
			
			UpdateContent();
			
			return true;
		}
		
		public void Focus()
		{
			if (_unloadCancellationTokenSource != null && _inFocusCollection.Add(this) && _previewEnabled)
				CancellableMessageDispatcher.SendMessageAsync(MessageType.PreviewScreenModeMakeReady, 3f, _unloadCancellationTokenSource.Token).Forget();
		}

		public void Unfocus()
		{
			if (_unloadCancellationTokenSource != null && _inFocusCollection.Remove(this) && _previewEnabled)
				CancellableMessageDispatcher.SendMessageAsync(MessageType.PreviewScreenModeMakeReady, 3f, _unloadCancellationTokenSource.Token).Forget();
		}
		
		public bool ContainsTabInHeader(WindowMaterialData material) => _header.ContainsTab(material);
		public bool AddTabToHeader(WindowMaterialData material) => _header.AddTab(material, material.GetIconType(), material.Name);
		public void ActivateTabInHeader(WindowMaterialData material) => _header.ActivateTab(material);
		public void ReorderTabsInHeader(IReadOnlyList<WindowMaterialData> orderedMaterials) => _header.ReorderTabs(orderedMaterials);
		public bool RemoveTabFromHeader(ulong materialId) => _header.RemoveTab(materialId);

		public override void Unload()
		{
			if (!_options.IsSumAdaptiveModeActive && _triggerSwitch != null)
			{
				_triggerSwitch.RemoveListener(_triggerKey, OnStatusColumnToggled);
				_triggerSwitch = null;
			}
			
			if (_unloadCancellationTokenSource is { IsCancellationRequested: false })
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
				_unloadCancellationTokenSource = null;
			}
			
			_header.Unload();
			
			_projectSettings = null;
			_inFocusCollection = null;
			_areaMaterial = null;
			
			OriginalParent = null;
			
			base.Unload();
		}

		private void UpdateContent()
		{
			if (_header.Visible)
			{
				var headerHeight = _header.RectTransform.rect.height;
				
				Content.sizeDelta = new Vector2(Content.sizeDelta.x, -headerHeight);
				Content.anchoredPosition = new Vector2(Content.anchoredPosition.x, -headerHeight * 0.5f);
			}
			else
			{
				Content.sizeDelta = new Vector2(Content.sizeDelta.x, 0f);
				Content.anchoredPosition = new Vector2(Content.anchoredPosition.x, 0f);
			}
		}
		
		private void ShowHeader()
		{
			if (!_options.IsSumAdaptiveModeActive && !_header.Visible)
				_header.Show();
		}
		
		private void HideHeader()
		{
			if (_header.Visible)
				_header.Hide();
		}
		
		private void OnStatusColumnToggled(bool visible)
		{
			if (_options.IsSumAdaptiveModeActive)
				return;
			
			if (visible)
				ShowHeader();
			else
				HideHeader();
			
			UpdateContent();
			
			MessageDispatcher.SendMessageData(MessageType.MaterialResize, _areaMaterial.StatusContent.ActiveMaterialId);
			
			if (_unloadCancellationTokenSource != null && _previewEnabled)
				CancellableMessageDispatcher.SendMessageAsync(MessageType.PreviewScreenModeMakeReady, 3f, _unloadCancellationTokenSource.Token).Forget();
		}
	}
}