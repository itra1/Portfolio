using System;
using System.Collections.Generic;
using System.Threading;
using Core.Base;
using Core.Utils;
using Cysharp.Threading.Tasks;
using UI.Switches.Triggers;
using UI.Switches.Triggers.Data;
using UI.Switches.Triggers.Data.Enums;
using UI.Switches.Triggers.Data.Enums.Attributes;
using UI.Switches.Triggers.Factory;
using Zenject;
using Debug = Core.Logging.Debug;

namespace UI.Switches
{
    public class TriggerSwitch : ITriggerSwitch, ICustomTriggerSwitch, ILateInitialized, ITickable, IDisposable
    {
        private readonly ITriggerFactory _triggerFactory;
        private readonly IDictionary<TriggerKey, ITrigger> _triggers;
        private readonly CancellationTokenSource _disposeCancellationTokenSource;
        
        public bool IsInitialized { get; private set; }
        
        public TriggerSwitch(ITriggerFactory triggerFactory)
        {
            _triggerFactory = triggerFactory;
            _triggers = new Dictionary<TriggerKey, ITrigger>();
            _disposeCancellationTokenSource = new CancellationTokenSource();
            
            CollectTriggersAsync().Forget();
        }
        
        public bool IsOn(in TriggerKey key) => 
            _triggers.TryGetValue(key, out var trigger) && trigger.Enabled;
        
        public bool HasListener(in TriggerKey key, Action<bool> listener) => 
            _triggers.TryGetValue(key, out var trigger) && trigger.Contains(listener);
        
        public bool AddListener(in TriggerKey key, Action<bool> listener)
        {
            if (_triggers.TryGetValue(key, out var trigger)) 
                return trigger.Add(listener);
            
            trigger = _triggerFactory.Create(key.Type);
            
            if (trigger == null) 
                return false;
            
            _triggers.Add(key, trigger);
            
            return trigger.Add(listener);
        }
        
        public bool RemoveListener(in TriggerKey key, Action<bool> listener)
        {
            if (!_triggers.TryGetValue(key, out var trigger))
                return false;
            
            if (!trigger.Remove(listener))
                return false;
            
            if (trigger.IsEmpty)
                _triggers.Remove(key);
            
            return true;
        }
        
        public bool Enable(in TriggerKey key)
        {
            if (!TryGetCustomTrigger(key, out var customTrigger))
                return false;
            
            customTrigger.Enable();
            return true;
        }
        
        public bool Disable(in TriggerKey key)
        {
            if (!TryGetCustomTrigger(key, out var customTrigger))
                return false;
            
            customTrigger.Disable();
            return true;
        }
        
        public bool Reset(in TriggerKey key)
        {
            if (!TryGetCustomTrigger(key, out var customTrigger))
                return false;
            
            customTrigger.Reset();
            return true;
        }
        
        public void Tick()
        {
            foreach (var trigger in _triggers.Values)
            {
                if (trigger.Charged) 
                    trigger.Fire();
            }
        }
        
        public void Dispose()
        {
            if (!_disposeCancellationTokenSource.IsCancellationRequested)
            {
                _disposeCancellationTokenSource.Cancel();
                _disposeCancellationTokenSource.Dispose();
            }
            
            foreach (var trigger in _triggers.Values)
                trigger.Dispose();
            
            _triggers.Clear();
        }
        
        private bool TryGetCustomTrigger(in TriggerKey key, out ICustomTrigger resultTrigger)
        {
            if (_triggers.TryGetValue(key, out var trigger) && trigger is ICustomTrigger customTrigger)
            {
                resultTrigger = customTrigger;
                return true;
            }
            
            resultTrigger = default;
            return false;
        }
        
        private async UniTaskVoid CollectTriggersAsync()
        {
            try
            {
                var cancellationToken = _disposeCancellationTokenSource.Token;
                
                await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
                {
                    await UniTask.SwitchToThreadPool();
                    
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    foreach (var value in Enum.GetValues(typeof(TriggerType)))
                    {
                        var type = (TriggerType) value;
                        var attribute = type.GetAttribute<TriggerAttribute>();
                        
                        if (attribute is not { IsStatic: true }) 
                            continue;
                        
                        var trigger = _triggerFactory.Create(type);
                        
                        if (trigger != null)
                            _triggers.Add(new TriggerKey(type), trigger);
                    }
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
            finally
            {
                if (Thread.CurrentThread.IsBackground)
                    await UniTask.SwitchToMainThread();
                
                IsInitialized = true;
            }
        }
    }
}