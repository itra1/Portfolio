using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base;
using Core.UI.Timers.Attributes;
using Core.UI.Timers.Data;
using Cysharp.Threading.Tasks;
using UI.Timers.Controller.Items.Base;
using Debug = Core.Logging.Debug;

namespace UI.Timers.Controller.Items.Factory
{
    public class TimersFactory : ITimersFactory
    {
        private readonly IOutgoingStateController _outgoingState;
        private readonly IOutgoingTimersTickStateController _outgoingTimersTickState;
        private readonly IDictionary<TimerType, Type> _timerTypes;
        private readonly CancellationTokenSource _disposeCancellationTokenSource;
        
        public bool IsInitialized { get; private set; }
        
        public IReadOnlyList<TimerType> Types => _timerTypes.Keys.ToArray();
        
        public TimersFactory(IOutgoingStateController outgoingState, IOutgoingTimersTickStateController outgoingTimersTickState)
        {
            _outgoingState = outgoingState;
            _outgoingTimersTickState = outgoingTimersTickState;
            _timerTypes = new Dictionary<TimerType, Type>();
            _disposeCancellationTokenSource = new CancellationTokenSource();
            
            CollectTimerTypesAsync().Forget();
        }

        public ITimer Create(TimerType type)
        {
            if (!_timerTypes.TryGetValue(type, out var timerType))
            {
                Debug.LogError($"Timer type is not found by type: {type}");
                return null;
            }
            
            return (ITimer) Activator.CreateInstance(timerType, _outgoingState, _outgoingTimersTickState);
        }

        public void Dispose()
        {
            if (!_disposeCancellationTokenSource.IsCancellationRequested)
            {
                _disposeCancellationTokenSource.Cancel();
                _disposeCancellationTokenSource.Dispose();
            }
            
            _timerTypes.Clear();
        }
        
        private async UniTaskVoid CollectTimerTypesAsync()
        {
            try
            {
                var cancellationToken = _disposeCancellationTokenSource.Token;
                
                await using (UniTask.ReturnToCurrentSynchronizationContext(cancellationToken: cancellationToken))
                {
                    await UniTask.SwitchToThreadPool();
					
                    var timerTypeBase = typeof(ITimer);
					
                    foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
						
                        if (!type.IsClass || type.IsAbstract || !timerTypeBase.IsAssignableFrom(type))
                            continue;
						
                        var attribute = type.GetCustomAttribute<TimerAttribute>();
						
                        if (attribute == null)
                            continue;
						
                        _timerTypes.Add(attribute.Type, type);
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