using Base;
using Core.Elements.Windows.Base.Data;
using Elements.Windows.Base;
using Materials.Presenter;
using UnityEngine;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Base
{
    public interface IWindowPresenterAdapter : IMaterialPresenter, ILockable,
        IWindowPresenterAdapterEventDispatcher, IWindowMaterialActionPerformer, IWindowStateSetter, IWindowContentDisplay
    {
        WindowMaterialData Material { get; }
        
        Vector2 OriginalContentSize { get; }
        
        bool IsInFullScreenMode { get; }

        void UpdateContent();
        
        void SetVideoStateInOutgoingState(string uuid);
        void RemoveVideoStateFromOutgoingState();
    }
}