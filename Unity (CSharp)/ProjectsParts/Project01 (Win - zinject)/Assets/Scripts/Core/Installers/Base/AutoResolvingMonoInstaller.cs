using System;
using Zenject;

namespace Core.Installers.Base
{
    public abstract class AutoResolvingMonoInstaller<TDerived> : MonoInstaller<TDerived>
        where TDerived : AutoResolvingMonoInstaller<TDerived>
    {
        public void Awake() => ResolveAll();
        
        public override void InstallBindings() { }
        
        protected FromBinderNonGeneric BindInterfacesTo<T>() => Container.BindInterfacesTo<T>();
        protected ConcreteIdBinderGeneric<TContract> Bind<TContract>() => Container.Bind<TContract>();
        protected ConcreteIdBinderNonGeneric Bind(params Type[] contractTypes) => Container.Bind(contractTypes);
        
        protected virtual void ResolveAll() { }
        
        protected TContract Resolve<TContract>() => Container.Resolve<TContract>();
    }
}