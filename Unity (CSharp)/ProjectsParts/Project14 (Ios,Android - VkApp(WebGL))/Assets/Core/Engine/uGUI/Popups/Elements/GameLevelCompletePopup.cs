using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.Audio;
using Core.Engine.Signals;

using UnityEngine.Events;

using Zenject;

namespace Core.Engine.uGUI.Popups.Elements
{
	/// <summary>
	/// Уровень окончен
	/// </summary>
	[PrefabName(PopupTypes.LevelComplete)]
	public class GameLevelCompletePopup : Popup
	{
		public UnityAction OnNextAction;
		public UnityAction OnExitAction;

		private SignalBus _signalBus;
		private int _winSoundIndex = -1;

		protected override float TimeShow => 0.5f;
		protected override float TimeHide => 0.5f;

		[Inject]
		public void Initiate(SignalBus signalBus)
		{
			_signalBus = signalBus;
		}

		private void OnEnable()
		{
			PlayAudio.PlaySound("Win");
		}

		protected override void BeforeShow()
		{
			base.BeforeShow();

			_winSoundIndex = ++_winSoundIndex % _soundLibrary.StartSounds.Length;

			_ = _audioFactory.Create()
			.AutoDespawn()
			.Play(_soundLibrary.WinSounds[_winSoundIndex]);
		}

		public void NextButtonTouch()
		{
			PlayAudio.PlaySound("click");
			OnNextAction?.Invoke();
			_ = Hide();
		}
		public void ExitButtonTouch()
		{
			PlayAudio.PlaySound("click");
			_ = Hide();
			_signalBus.Fire<GameExitPlaySignal>();
			OnExitAction?.Invoke();
		}

	}
}
