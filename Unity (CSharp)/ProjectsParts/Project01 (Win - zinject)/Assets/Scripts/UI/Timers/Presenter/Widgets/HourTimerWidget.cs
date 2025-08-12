using System;
using Core.UI.Timers.Data;
using TMPro;
using UnityEngine;

namespace UI.Timers.Presenter.Widgets
{
    public class HourTimerWidget : ClockFaceWidgetBase
    {
        [SerializeField] private TextMeshProUGUI _hoursLabel;
        [SerializeField] private TextMeshProUGUI _minutesLabel;
        [SerializeField] private TextMeshProUGUI _secondsLabel;
        
        public override TimerType Type => TimerType.Hour;
        
        public override void Refresh()
        {
            var running = Info.Running;
            
            base.Refresh();
            
            if (running)
            {
                var timeSpan = TimeSpan.FromSeconds(Math.Abs(Info.CurrentTime));
            
                _hoursLabel.text = timeSpan.Hours.ToString(DefaultLabelText);
                _minutesLabel.text = timeSpan.Minutes.ToString(DefaultLabelText);
                _secondsLabel.text = timeSpan.Seconds.ToString(DefaultLabelText);
            }
        }
        
        protected override void Reset()
        {
            base.Reset();
            
            _hoursLabel.text = DefaultLabelText;
            _minutesLabel.text = DefaultLabelText;
            _secondsLabel.text = DefaultLabelText;
        }
    }
}