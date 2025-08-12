using System;
using System.Collections.Generic;
using Zenject;

namespace Core.Installers.Base
{
    public abstract class AutoResolvingInstaller : Installer
    {
        private readonly ISet<IDisposable> _disposables;
        
        protected AutoResolvingInstaller() => _disposables = new HashSet<IDisposable>();
        
        public override void InstallBindings() => ResolveAll();
        
        protected FromBinderNonGeneric BindInterfacesTo<T>() => Container.BindInterfacesTo<T>();
        protected ConcreteIdBinderGeneric<TContract> Bind<TContract>() => Container.Bind<TContract>();
        protected ConcreteIdBinderNonGeneric Bind(params Type[] contractTypes) => Container.Bind(contractTypes);
        
        protected virtual void ResolveAll()
        {
            var disposableManager = Container.Resolve<DisposableManager>();
            
            foreach (var disposable in _disposables)
                disposableManager.Add(disposable);
            
            _disposables.Clear();
        }
        
        protected TContract Resolve<TContract>()
        {
            var contract = Container.Resolve<TContract>();
            
            if (contract is IDisposable disposable)
                _disposables.Add(disposable);
            
            return contract;
        }
    }
}