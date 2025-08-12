using System;
using Core.UI.Timers.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Timers.Presenter.Widgets
{
    public class StopwatchWidget : TimerWidgetBase
    {
        [SerializeField] private Image _progress;
        [SerializeField] private TextMeshProUGUI _hoursLabel;
        [SerializeField] private TextMeshProUGUI _minutesLabel;
        [SerializeField] private TextMeshProUGUI _secondsLabel;
        [SerializeField] private Color _inactiveColor;
        [SerializeField] private Color _activeColor;
        
        public override TimerType Type => TimerType.Stopwatch;
        
        public override void Refresh()
        {
            var running = Info.Running;
            
            UpdatePosition();
            UpdateColor();
            
            if (running)
            {
                _progress.fillAmount = 1f;
                
                SetColor(_inactiveColor);
                
                var timeSpan = TimeSpan.FromSeconds(Math.Abs(Info.CurrentTime));
                
                _hoursLabel.text = timeSpan.Hours.ToString(DefaultLabelText);
                _minutesLabel.text = timeSpan.Minutes.ToString(DefaultLabelText);
                _secondsLabel.text = timeSpan.Seconds.ToString(DefaultLabelText);
            }
            else
            {
                Reset();
            }
        }
        
        protected override void Reset()
        {
            _progress.fillAmount = 0f;
            
            SetColor(_inactiveColor);
            
            _hoursLabel.text = DefaultLabelText;
            _minutesLabel.text = DefaultLabelText;
            _secondsLabel.text = DefaultLabelText;
        }
    }
}