using System;
using Cysharp.Threading.Tasks;
using UI.ShadedElements.Presenter.Targets.Base;

namespace UI.ShadedElements.Base
{
    public interface IInFocusCollection : IAnimationOptions
    {
        bool Contains(IFocusCapable target);
        bool Add(IFocusCapable target, Action onFocused = null, Func<bool, UniTask> onUnfocusedAsync = null);
        bool Remove(IFocusCapable target);
    }
}