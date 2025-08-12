using System;
using System.Threading;
using Base.Presenter;
using Core.Elements.Windows.Base.Data;
using Core.Materials.Data;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Cysharp.Threading.Tasks;
using Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components;
using Elements.Windows.Base.Presenter;
using Settings;
using UI.FullScreen.Presenter;
using UI.FullScreen.Presenter.Targets.Base;
using UI.Switches;
using UI.Switches.Triggers.Data.Enums;
using UnityEngine;
using Zenject;
using Debug = Core.Logging.Debug;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Base
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public abstract class WindowPresenterAdapterBase<TWindowPresenter> : MonoBehaviour, IWindowPresenterAdapter
        where TWindowPresenter : IWindowPresenter
    {
        [SerializeField] private TWindowPresenter _adaptee;
        [SerializeField] private GrabArea _grabArea;
        
        private ITriggerSwitch _triggerSwitch;
        private IWindowFullScreenState _fullScreenState;
        private CancellationTokenSource _updateCancellationTokenSource;
        
        protected TWindowPresenter Adaptee => _adaptee;
        
        protected IUISettings Settings { get; private set; }
        protected IOutgoingStateController OutgoingState { get; private set; }
        
        protected bool IsTriggerOn => _triggerSwitch.IsOn(_adaptee.TriggerKey);

        protected bool Locked { get; private set; }
        
        public event Action StateChanged;
        
        public event Action DragStarted
        {
            add => _grabArea.DragStarted += value;
            remove => _grabArea.DragStarted -= value;
        }
        
        public event Action Drag
        {
            add => _grabArea.Drag += value;
            remove => _grabArea.Drag -= value;
        }
        
        public event Action DragStopped 
        {
            add => _grabArea.DragStopped += value;
            remove => _grabArea.DragStopped -= value;
        }
        
        public event Action Closed;
        
        public event Action ContentDisplayed
        {
            add => _adaptee.ContentDisplayed += value;
            remove => _adaptee.ContentDisplayed -= value;
        }
        
        public WindowMaterialData Material { get; private set; }
        
        public RectTransform RectTransform => _adaptee.RectTransform;
        public RectTransform Content => _adaptee.Content;
        
        public bool Visible => _adaptee.Visible;
        
        public bool IsContentDisplayed => _adaptee.IsContentDisplayed;
        
        public virtual Vector2 OriginalContentSize => Content.rect.size;
        
        public bool IsInFullScreenMode => _adaptee is IFullScreenCapable target && _fullScreenState.IsInFullScreenMode(target);

        [Inject]
        private void Initialize(DiContainer container, 
            ITriggerSwitch triggerSwitch,
            IWindowFullScreenToggle fullScreenToggle,
            IUISettings settings,
            IOutgoingStateController outgoingState)
        {
            container.Inject(_adaptee);
            
            _triggerSwitch = triggerSwitch;
            _fullScreenState = fullScreenToggle;
            
            Settings = settings;
            OutgoingState = outgoingState;
        }
        
        public void SetParentArea(IParentArea parentArea) => _adaptee.SetParentArea(parentArea);

        public bool SetMaterial(MaterialData material)
        {
            if (!_adaptee.SetMaterial(material))
                return false;
            
            Material = (WindowMaterialData) material;
            return true;
        }
        
        public virtual void SetParentOnInitialize(RectTransform parent) => _adaptee.SetParentOnInitialize(parent);
        
        public void AlignToParent() => _adaptee.AlignToParent();
        
        public bool SetState(WindowState state) => _adaptee.SetState(state);
        
        public void PerformAction(string actionAlias, ulong materialId) => _adaptee.PerformAction(actionAlias, materialId);
        
        public virtual void UpdateContent() { }
        
        public virtual void SetVideoStateInOutgoingState(string uuid) => 
            OutgoingState.Context.SetFloatingVideoWindowState(uuid, Material, false, false, 0);
        
        public virtual void RemoveVideoStateFromOutgoingState() => 
            OutgoingState.Context.RemoveFloatingVideoWindowState(Material);
        
        public virtual UniTask<bool> PreloadAsync()
        {
            var triggerKey = _adaptee.TriggerKey;
            
            if (triggerKey == default || triggerKey.Type == TriggerType.Default)
                Debug.LogError($"The trigger key is not set or is set with a default type value when trying to preload presenter adapter with material {Material}");
            
            _triggerSwitch.AddListener(triggerKey, OnWindowPanelsToggled);
            
            return _adaptee.PreloadAsync();
        }

        public virtual bool Show()
        {
            if (!_adaptee.Show())
                return false;
            
            EnableComponents();
            UpdateAsync().Forget();
            return false;
        }
        
        public virtual void Lock() => Locked = true;
        public virtual void Unlock() => Locked = false;
        
        public virtual bool Hide()
        {
            if (!_adaptee.Hide())
                return false;
            
            Locked = false;
            
            if (_updateCancellationTokenSource != null)
            {
                _updateCancellationTokenSource.Cancel();
                _updateCancellationTokenSource.Dispose();
                _updateCancellationTokenSource = null;
            }
            
            DisableComponents();
            return true;
        }
        
        public virtual void Unload()
        {
            try
            {
                if (_updateCancellationTokenSource != null)
                {
                    _updateCancellationTokenSource.Cancel();
                    _updateCancellationTokenSource.Dispose();
                    _updateCancellationTokenSource = null;
                }
                
                if (_triggerSwitch != null)
                {
                    _triggerSwitch.RemoveListener(_adaptee.TriggerKey, OnWindowPanelsToggled);
                    _triggerSwitch = null;
                }

                _fullScreenState = null;
                
                DisableComponents();
                
                _adaptee.Unload();
            }
            catch
            {
                // ignored
            }
            
            Settings = null;
            OutgoingState = null;
            
            Material = null;
            
            Locked = false;
        }
        
        private async UniTaskVoid UpdateAsync()
        {
            _updateCancellationTokenSource = new CancellationTokenSource();
			
            try
            {
                var cancellationToken = _updateCancellationTokenSource.Token;
				
                while (_updateCancellationTokenSource != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    {
                        if (!_grabArea.Visible)
                            _grabArea.Show();
                    }
                    else
                    {
                        if (_grabArea.Visible)
                            _grabArea.Hide();
                    }
                    
                    await UniTask.NextFrame(cancellationToken);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }
        
        protected void DispatchStateChangedEvent() => StateChanged?.Invoke();
        protected void DispatchClosedEvent() => Closed?.Invoke();
        
        protected virtual void EnableComponents() { }
        
        protected virtual void DisableComponents() { }
        
        protected virtual void OnWindowPanelsToggled(bool visible) { }
    }
}