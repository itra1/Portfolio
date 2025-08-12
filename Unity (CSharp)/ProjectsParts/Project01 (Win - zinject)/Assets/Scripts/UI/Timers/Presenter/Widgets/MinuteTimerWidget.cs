using System;
using Core.UI.Timers.Data;
using TMPro;
using UnityEngine;

namespace UI.Timers.Presenter.Widgets
{
    public class MinuteTimerWidget : ClockFaceWidgetBase
    {
        [SerializeField] private TextMeshProUGUI _minutesLabel;
        [SerializeField] private TextMeshProUGUI _secondsLabel;
        
        public override TimerType Type => TimerType.Minute;
        
        public override void Refresh()
        {
            var running = Info.Running;
            
            base.Refresh();
            
            if (running)
            {
                var timeSpan = TimeSpan.FromSeconds(Math.Abs(Info.CurrentTime));
            
                _minutesLabel.text = timeSpan.Minutes.ToString(DefaultLabelText);
                _secondsLabel.text = timeSpan.Seconds.ToString(DefaultLabelText);
            }
        }
        
        protected override void Reset()
        {
            base.Reset();
            
            _minutesLabel.text = DefaultLabelText;
            _secondsLabel.text = DefaultLabelText;
        }
    }
}