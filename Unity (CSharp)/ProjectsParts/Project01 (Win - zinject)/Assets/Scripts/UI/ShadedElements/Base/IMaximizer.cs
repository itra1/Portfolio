using System;
using Cysharp.Threading.Tasks;
using UI.ShadedElements.Presenter.Targets.Base;

namespace UI.ShadedElements.Base
{
    public interface IMaximizer : IAnimationOptions
    {
        bool IsMaximized(IMaximizable target);
        bool Maximize(IMaximizable target, Action onMaximized = null, Func<bool, UniTask> onSizeRestoredAsync = null);
        bool RestoreSize(IMaximizable target);
    }
}