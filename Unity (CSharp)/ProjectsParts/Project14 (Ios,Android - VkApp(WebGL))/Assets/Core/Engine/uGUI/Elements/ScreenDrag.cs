using Core.Engine.Signals;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Core.Engine.uGUI.Elements
{
	public class ScreenDrag :MonoBehaviour, IDragHandler, IZInjection
	{
		private SignalBus _signalBus;

		[Inject]
		public void Initiate(SignalBus signalBud)
		{
			_signalBus = signalBud;
		}
		public void OnDrag(PointerEventData eventData)
		{
			_signalBus.Fire(new ScreenDragSignal() { DragDelta = eventData.delta });
		}
	}
}
