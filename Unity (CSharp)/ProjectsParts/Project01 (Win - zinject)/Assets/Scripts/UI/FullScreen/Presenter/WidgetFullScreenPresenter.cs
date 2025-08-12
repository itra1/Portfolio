using System;
using Base.Presenter;
using com.ootii.Messages;
using Core.Materials.Data;
using Core.Messages;
using Cysharp.Threading.Tasks;
using Elements.Windows.Base.Presenter;
using Elements.Windows.Common.Presenter.Factory;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Debug = Core.Logging.Debug;

namespace UI.FullScreen.Presenter
{
    public class WidgetFullScreenPresenter : PresenterBase, IWidgetFullScreenToggle, IDisposable
    {
        [SerializeField] private Image _background;
        [SerializeField] private Button _button;
        
        public event Action Clicked;
        
        private IWindowPresenterFactory _presenterFactory;
        private IWindowPresenter _currentWindow;
        private MaterialData _currentMaterial;
        
        [Inject]
        private void Initialize(IWindowPresenterFactory presenterFactory)
        {
            _presenterFactory = presenterFactory;
            
            SetName("WidgetFullScreen");
            
            _background.enabled = false;
            _button.enabled = false;
            
            Content.gameObject.SetActive(false);
            
            MessageDispatcher.AddListener(MessageType.AppInitialize, OnApplicationInitialized);
        }

        public override void AlignToParent()
        {
            base.AlignToParent();
            
            var content = Content;
            var contentWidth = Content.rect.width;
            
            content.sizeDelta = new Vector2(-contentWidth, content.sizeDelta.y);
        }

        public void Toggle(MaterialData material)
        {
            if (_currentMaterial == material)
            {
                RemoveWindow();
            }
            else
            {
                if (_currentMaterial != null)
                    RemoveWindow();
                
                CreateWindowAsync(material).Forget();
            }
        }

        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            if (_currentMaterial != null)
                RemoveWindow();
            
            _presenterFactory = null;
        }

        private async UniTaskVoid CreateWindowAsync(MaterialData material)
        {
            var presenter = _presenterFactory.Create(material, Content);
            
            if (presenter == null)
            {
                Debug.LogError($"Failed to instantiate the MaterialPresenter by material {material}");
                return;
            }
            
            if (!presenter.SetMaterial(material) || !await presenter.PreloadAsync())
            {
                presenter.Unload();
                return;
            }
            
            Content.gameObject.SetActive(true);
            
            presenter.AlignToParent();
            presenter.Show();
            
            _currentWindow = presenter;
            _currentMaterial = material;

            if (!_background.enabled)
            {
                _background.enabled = true;
            
                _button.enabled = true;
                _button.onClick.AddListener(OnButtonClicked);
            }
        }
        
        private void RemoveWindow()
        {
            if (_background.enabled)
            {
                _button.onClick.RemoveListener(OnButtonClicked);
                _button.enabled = false;
            
                _background.enabled = false;
            }
            
            _currentMaterial = null;
            
            _currentWindow.Unload();
            _currentWindow = null;
            
            Content.gameObject.SetActive(false);
        }
        
        private void OnButtonClicked() => Clicked?.Invoke();
        
        private void OnApplicationInitialized(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            AlignToParent();
            
            if (!gameObject.activeSelf) 
                Show();
        }
    }
}