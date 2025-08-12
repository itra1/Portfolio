using System;
using TMPro;
using UI.Profiling.Presenter.Base;
using UnityEngine;

namespace UI.Profiling.Presenter
{
    [DisallowMultipleComponent]
    public class FpsCounterPresenter : ProfilerItemPresenterBase
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private int _frameDeltaTimeValuesCount;
        
        private float[] _frameDeltaTimeValues;
        private int _lastIndex;
        
        public override void Deactivate()
        {
            base.Deactivate();
                
            _label.text = string.Empty;
            
            Array.Fill(_frameDeltaTimeValues, default);
            _lastIndex = 0;
        }
        
        public void Awake() => _frameDeltaTimeValues = new float[_frameDeltaTimeValuesCount];
        
        private void Update()
        {
            if (_frameDeltaTimeValues == null)
                return;
            
            _frameDeltaTimeValues[_lastIndex] = Time.deltaTime;
            _lastIndex = (_lastIndex + 1) % _frameDeltaTimeValuesCount;
            
            _label.text = Mathf.RoundToInt(CalculateFpsValue()).ToString();
        }
        
        private float CalculateFpsValue()
        {
            var valuesSum = 0f;
            
            for (var i = 0; i < _frameDeltaTimeValuesCount; i++)
                valuesSum += _frameDeltaTimeValues[i];
            
            return _frameDeltaTimeValuesCount / valuesSum;
        }
    }
}