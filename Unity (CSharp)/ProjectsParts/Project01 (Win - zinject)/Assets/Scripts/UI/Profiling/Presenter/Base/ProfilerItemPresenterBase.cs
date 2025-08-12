using UnityEngine;

namespace UI.Profiling.Presenter.Base
{
    [DisallowMultipleComponent]
    public abstract class ProfilerItemPresenterBase : MonoBehaviour, IProfilerItemPresenter
    {
        public bool Active => gameObject.activeSelf;
        
        public virtual void Activate() => gameObject.SetActive(true);

        public virtual void Deactivate() => gameObject.SetActive(false);
    }
}