using System;
using Elements.FloatingWindow.Presenter.Enums;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components
{
	[DisallowMultipleComponent]
	public class ResizingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
		IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[SerializeField] private WindowCorner _corner;
		
		public event Action<WindowCorner> ResizeStarted;
		public event Action<WindowCorner> Resize;
		public event Action<WindowCorner> ResizeStopped;
		
		public void OnPointerDown(PointerEventData eventData) => ResizeStarted?.Invoke(_corner);
		public void OnBeginDrag(PointerEventData eventData) => ResizeStarted?.Invoke(_corner);
		
		public void OnDrag(PointerEventData eventData) => Resize?.Invoke(_corner);
		
		public void OnPointerUp(PointerEventData eventData) => ResizeStopped?.Invoke(_corner);
		public void OnEndDrag(PointerEventData eventData) => ResizeStopped?.Invoke(_corner);
	}
}
