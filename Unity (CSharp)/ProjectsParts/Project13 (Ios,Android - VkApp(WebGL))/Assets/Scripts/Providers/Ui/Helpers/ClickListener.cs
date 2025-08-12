using Game.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.Providers.Ui.Helpers
{
	public class ClickListener : MonoBehaviour, IInjection, IPointerDownHandler
	{
		public UnityEvent OnScreenClickEvent = new();

		public void OnPointerDown(PointerEventData eventData)
		{
			OnScreenClickEvent?.Invoke();
			//_signalBus.Fire(new GamePointerDownSignal(eventData));
		}
	}
}
