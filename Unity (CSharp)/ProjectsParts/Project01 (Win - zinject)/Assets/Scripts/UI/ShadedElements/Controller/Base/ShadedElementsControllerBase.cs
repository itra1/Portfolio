using System;
using Base.Presenter;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using UI.ShadedElements.Base;
using UI.ShadedElements.Presenter.Targets.Base;
using UnityEngine;

namespace UI.ShadedElements.Controller.Base
{
    public abstract class ShadedElementsControllerBase<TPresenter> where TPresenter : PresenterBase, IPresenter, IInFocusCollection
    {
        private readonly IPresenterFactory _presenterFactory;

        public float AnimationDuration => Presenter.AnimationDuration;
        
        protected TPresenter Presenter { get; private set; }
        
        protected ShadedElementsControllerBase(IPresenterFactory presenterFactory) => 
            _presenterFactory = presenterFactory;
        
        public bool Preload(RectTransform parent)
        {
            Presenter = _presenterFactory.Create<TPresenter>(parent);
            
            if (Presenter == null)
            {
                Debug.LogError($"Failed to instantiate the {GetType().Name}");
                return false;
            }
            
            Presenter.AlignToParent();
            Presenter.Show();
            
            return true;
        }
        
        public bool Contains(IFocusCapable target) => Presenter != null && Presenter.Contains(target);
        
        public bool Add(IFocusCapable target, Action onFocused = null, Func<bool, UniTask> onUnfocusedAsync = null) => 
            Presenter != null && Presenter.Add(target, onFocused, onUnfocusedAsync);
        
        public bool Remove(IFocusCapable target) => Presenter != null && Presenter.Remove(target);
        
        public void Dispose()
        {
            if (Presenter != null)
            {
                Presenter.Unload();
                Presenter = null;
            }
        }
    }
}