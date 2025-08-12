using System;
using Elements.FloatingWindow.Presenter.Enums;
using UnityEngine;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components
{
    [DisallowMultipleComponent]
    public class ResizingArea : MonoBehaviour
    {
        [SerializeField] private ResizingButton[] _buttons;
        
        public event Action<WindowCorner> ResizeStarted
        {
            add
            {
                for (var i = _buttons.Length - 1; i >= 0; i--)
                    _buttons[i].ResizeStarted += value;
            }
            remove
            {
                for (var i = _buttons.Length - 1; i >= 0; i--)
                    _buttons[i].ResizeStarted -= value;
            }
        }

        public event Action<WindowCorner> Resize
        {
            add
            {
                for (var i = _buttons.Length - 1; i >= 0; i--)
                    _buttons[i].Resize += value;
            }
            remove
            {
                for (var i = _buttons.Length - 1; i >= 0; i--)
                    _buttons[i].Resize -= value;
            }
        }
        
        public event Action<WindowCorner> ResizeStopped
        {
            add
            {
                for (var i = _buttons.Length - 1; i >= 0; i--)
                    _buttons[i].ResizeStopped += value;
            }
            remove
            {
                for (var i = _buttons.Length - 1; i >= 0; i--)
                    _buttons[i].ResizeStopped -= value;
            }
        }
    }
}
