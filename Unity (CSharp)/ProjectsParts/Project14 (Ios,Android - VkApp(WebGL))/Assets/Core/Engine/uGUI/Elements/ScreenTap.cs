using Core.Engine.Signals;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

namespace Core.Engine.uGUI
{
	public class ScreenTap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IZInjection
	{
		private SignalBus _signalBus;
		private bool _touchIdentified;
		[Inject]
		public void Initiate(SignalBus signalBud)
		{
			_signalBus = signalBud;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_touchIdentified = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (!_touchIdentified) return;

			_touchIdentified = false;
			Vector3 clickPositionWord = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10));
			_signalBus.Fire(new ScreenTapSignal() { Position = clickPositionWord });
		}
	}
}
