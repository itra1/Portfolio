using System;
using Cysharp.Threading.Tasks;

namespace Base
{
    public interface IVisualAsync
    {
        UniTask<bool> ShowAsync(Action onStarted = null, Action onFinished = null);
        UniTask<bool> HideAsync(Action onStarted = null, Action onFinished = null);
    }
}