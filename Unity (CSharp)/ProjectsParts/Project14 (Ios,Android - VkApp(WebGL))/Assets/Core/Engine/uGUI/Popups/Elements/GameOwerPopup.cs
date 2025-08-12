using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.Audio;
using Core.Engine.Signals;
using UnityEngine.Events;
using Zenject;

namespace Core.Engine.uGUI.Popups
{
	/// <summary>
	/// попап гейм овера
	/// </summary>
	[PrefabName(PopupTypes.GameOwer)]
	public class GameOwerPopup : Popup
	{
		private SignalBus _signalBus;

		public UnityAction OnRepeatAction;
		public UnityAction OnExitAction;

		[Inject]
		public void Initiate(SignalBus signalBus)
		{
			_signalBus = signalBus;
		}

		private void OnEnable()
		{
			PlayAudio.PlaySound("GameOwer");
		}

		/// <summary>
		/// Кнопка повторения
		/// </summary>
		public void RepeatButtonTouch()
		{
			PlayAudio.PlaySound("click");
			_signalBus.Fire<GameRepeatPlaySignal>();
			OnRepeatAction?.Invoke();
			_ = Hide();
		}

		/// <summary>
		/// Кнопка повторения
		/// </summary>
		public void ExitButtonTouch()
		{
			PlayAudio.PlaySound("click");
			_signalBus.Fire<GameExitPlaySignal>();
			OnExitAction?.Invoke();
			_ = Hide();
		}
	}

}

