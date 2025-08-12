using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.Scripts.UI.Components
{
	public class CarouselSideButton : MonoBehaviour, IPointerDownHandler
	{
		[HideInInspector] public UnityEvent OnPointDown = new();
		public void OnPointerDown(PointerEventData eventData)
		{
			OnPointDown?.Invoke();
		}
	}
}
