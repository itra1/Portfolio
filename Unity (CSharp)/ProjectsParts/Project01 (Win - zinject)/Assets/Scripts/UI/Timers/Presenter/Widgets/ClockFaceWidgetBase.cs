using Core.UI.Timers.Data;
using UI.Timers.Presenter.Widgets.Info;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Timers.Presenter.Widgets
{
    public abstract class ClockFaceWidgetBase : TimerWidgetBase
    {
        [SerializeField] private Image _progress;
        [SerializeField] private RectTransform _face;
        [SerializeField] private RectTransform _minusSign;
        [SerializeField] private Color _inactiveColor;
        [SerializeField] private ActiveColorInfo _activeColorFull;
        [SerializeField] private ActiveColorInfo _activeColorHalf;
        [SerializeField] private ActiveColorInfo _activeColorQuarter;
        [SerializeField] private ActiveColorInfo _activeColorEmpty;
        
        public override TimerType Type => TimerType.Hour;
        
        public override void Refresh()
        {
            var running = Info.Running;
            
            UpdatePosition();
            UpdateColor();

            if (running)
            {
                var currentTime = Info.CurrentTime;
                var totalTime = Info.TotalTime;
                
                Color color;
                
                if (currentTime < 0)
                {
                    _minusSign.gameObject.SetActive(true);
                    _face.localPosition = Vector3.right * _minusSign.rect.width * 0.5f;
                    _progress.fillAmount = 1f;
                    
                    color = _activeColorEmpty.Color;
                }
                else
                {
                    var progress = Mathf.Clamp01((float) currentTime / totalTime);
                    
                    _minusSign.gameObject.SetActive(false);
                    _face.localPosition = Vector3.zero;
                    _progress.fillAmount = progress;
                    
                    var fullProgress = _activeColorFull.Progress;
                    var halfProgress = _activeColorHalf.Progress;
                    var quarterProgress = _activeColorQuarter.Progress;
                    var emptyProgress = _activeColorEmpty.Progress;
                    
                    if (progress <= fullProgress && progress > halfProgress)
                    {
                        color = Color.Lerp(_activeColorFull.Color, _activeColorHalf.Color, 
                            (progress - fullProgress) / (halfProgress - fullProgress));
                    }
                    else if (progress <= halfProgress && progress > quarterProgress)
                    {
                        color = Color.Lerp(_activeColorHalf.Color, _activeColorQuarter.Color, 
                            (progress - halfProgress) / (quarterProgress - halfProgress));
                    }
                    else if (progress <= quarterProgress && progress > emptyProgress)
                    {
                        color = Color.Lerp(_activeColorQuarter.Color, _activeColorEmpty.Color, 
                            (progress - quarterProgress) / (emptyProgress - quarterProgress));
                    }
                    else
                    {
                        color = _activeColorEmpty.Color;
                    }
                }
                
                SetColor(color);
            }
            else
            {
                Reset();
            }
        }

        protected override void Reset()
        {
            _minusSign.gameObject.SetActive(false);
            _face.localPosition = Vector3.zero;
            _progress.fillAmount = 0f;
            
            SetColor(_inactiveColor);
        }
    }
}