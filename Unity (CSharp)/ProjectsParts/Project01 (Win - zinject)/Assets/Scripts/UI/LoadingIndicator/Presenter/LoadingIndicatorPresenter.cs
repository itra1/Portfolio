using System.Collections.Generic;
using com.ootii.Messages;
using Core.Messages;
using UnityEngine;
using UnityEngine.UI;

namespace UI.LoadingIndicator.Presenter
{
    [DisallowMultipleComponent]
    public class LoadingIndicatorPresenter : MonoBehaviour, ILoadingIndicatorPresenter
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _inProgressState;
        [SerializeField] private Color _idleState;
        
        private Color _defaultColor;
        private ISet<object> _initiators;
        
        public bool Active => gameObject.activeSelf;
        
        public void Initialize()
        {
            _defaultColor = _image.color;
            _initiators = new HashSet<object>();
        }
        
        public void Activate()
        {
            if (Active)
                return;
            
            gameObject.SetActive(true);
            
            if (_image.color == _defaultColor) 
                _image.color = _idleState;
            
            MessageDispatcher.AddListener(MessageType.AppStart, OnApplicationStarted);
            MessageDispatcher.AddListener(MessageType.AppLoadComplete, OnApplicationCompleted);
            MessageDispatcher.AddListener(MessageType.LoadingStateChange, OnLoadingStateChanged);
        }
        
        public void Deactivate()
        {
            if (!Active)
                return;
            
            MessageDispatcher.RemoveListener(MessageType.AppStart, OnApplicationStarted);
            MessageDispatcher.RemoveListener(MessageType.AppLoadComplete, OnApplicationCompleted);
            MessageDispatcher.RemoveListener(MessageType.LoadingStateChange, OnLoadingStateChanged);
            
            _image.color = _defaultColor;
            
            gameObject.SetActive(false);
        }
        
        public void Unload()
        {
            _initiators.Clear();
            _initiators = null;
            
            _defaultColor = default;
        }

        private void SetState(bool inProgress, object initiator = null)
        {
            if (_initiators == null)
                return;
            
            if (inProgress)
            {
                if (initiator != null)
                    _initiators.Add(initiator);
                
                _image.color = _inProgressState;
            }
            else
            {
                if (initiator != null)
                    _initiators.Remove(initiator);
                
                if (_initiators.Count == 0)
                    _image.color = _idleState;
            }
        }
        
        private void OnApplicationStarted(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppStart, OnApplicationStarted);
            SetState(true);
        }

        private void OnApplicationCompleted(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppLoadComplete, OnApplicationCompleted);
            SetState(false);
        }

        private void OnLoadingStateChanged(IMessage message) => SetState((bool) message.Data, message.Sender);
    }
}