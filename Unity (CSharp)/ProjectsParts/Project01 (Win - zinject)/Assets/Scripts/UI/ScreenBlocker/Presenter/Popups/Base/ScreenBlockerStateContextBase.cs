using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace UI.ScreenBlocker.Presenter.Popups.Base
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public abstract class ScreenBlockerStateContextBase : ScreenBlockerPopupBase, IScreenBlockerStateContext
    {
        private IDictionary<Type, IScreenBlockerState> _states;
        private IScreenBlockerState _currentState;
        
        protected void ConfigureStates() => 
            _states = new Dictionary<Type, IScreenBlockerState>();
        
        protected void RegisterState(IScreenBlockerState state) =>
            _states.Add(state.GetType(), state);
        
        public void ChangeState<TScreenBlockerState>() where TScreenBlockerState : IScreenBlockerState
        {
            if (!_states.TryGetValue(typeof(TScreenBlockerState), out var state))
            {
                Debug.LogError($"{typeof(TScreenBlockerState).Name} is not registered in {GetType().Name}");
                return;
            }
            
            if (_currentState != null)
            {
                _currentState.Hide();
                _currentState = null;
            }
            
            _currentState = state;
            _currentState.Show();
        }
        
        public override void Dispose()
        {
            if (_currentState != null)
            {
                _currentState.Hide();
                _currentState = null;
            }
            
            _states.Clear();
        }
    }
}