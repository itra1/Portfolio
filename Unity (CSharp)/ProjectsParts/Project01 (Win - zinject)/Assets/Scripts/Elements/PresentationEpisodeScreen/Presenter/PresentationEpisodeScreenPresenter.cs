using System.Threading;
using Base.Presenter;
using Core.Materials.Data;
using Core.Messages;
using Core.Settings;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Preview;
using UI.ShadedElements.Base;
using UI.ShadedElements.Controller;
using UI.ShadedElements.Presenter.Targets.Base;
using UnityEngine;
using Zenject;

namespace Elements.PresentationEpisodeScreen.Presenter
{
    public class PresentationEpisodeScreenPresenter : PresenterBase, IPresentationEpisodeScreenPresenter, IFocusCapable
    {
        private IProjectSettings _projectSettings;
        private IInFocusCollection _inFocusCollection;
        private CancellationTokenSource _unloadCancellationTokenSource;

        private ContentAreaMaterialData _areaMaterial;
        private bool _previewEnabled;

        public ulong? AreaId => _areaMaterial?.Id;
        
        public RectTransform OriginalParent { get; private set; }
        public bool InFocus => _inFocusCollection.Contains(this);

        [Inject]
        private void Initialize(IProjectSettings projectSettings,
            IPreviewState previewState,
            IShadedScreenModesController inFocusCollection)
        {
            _projectSettings = projectSettings;
            _inFocusCollection = inFocusCollection;
            _unloadCancellationTokenSource = new CancellationTokenSource();
            _previewEnabled = previewState.Enabled;
        }
        
        public bool SetMaterial(ContentAreaMaterialData areaMaterial)
        {
            if (areaMaterial == null)
            {
                Debug.LogError("An attempt was detected to assign a null area material to the PresentationEpisodeScreenPresenter");
                return false;
            }

            if (_areaMaterial != null)
            {
                Debug.LogError("The area material has already been assigned before. Not allowed to reassign area material to the PresentationEpisodeScreenPresenter");
                return false;
            }

            _areaMaterial = areaMaterial;

            SetName($"Screen: {areaMaterial.Id}");
            
            return true;
        }
        
        public override void SetParentOnInitialize(RectTransform parent)
        {
            OriginalParent = parent;
            base.SetParentOnInitialize(parent);
        }

        public override void AlignToParent()
        {
            if (_areaMaterial == null)
                return;
            
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

        public override void Unload()
        {
            if (_unloadCancellationTokenSource is { IsCancellationRequested: false })
            {
                _unloadCancellationTokenSource.Cancel();
                _unloadCancellationTokenSource.Dispose();
                _unloadCancellationTokenSource = null;
            }
            
            _projectSettings = null;
            _inFocusCollection = null;
            
            OriginalParent = null;
            
            _areaMaterial = null;
            
            base.Unload();
        }
    }
}