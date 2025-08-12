using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Garilla.Main
{
	public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public UnityEngine.Events.UnityEvent PointerEnterEvent = new UnityEngine.Events.UnityEvent();
		public UnityEngine.Events.UnityEvent PointerExitEvent = new UnityEngine.Events.UnityEvent();

		public void OnPointerEnter(PointerEventData eventData)
		{
			PointerEnterEvent?.Invoke();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			PointerExitEvent?.Invoke();
		}
	}
}