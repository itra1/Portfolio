using Game.Providers.Ui.Elements;
using Game.Providers.Ui.Presenters.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.Providers.Ui.Presenters
{
	public class GameSelectWindowPresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent OnSoloSelect = new();
		[HideInInspector] public UnityEvent OnDuelSelect = new();

		[SerializeField] private GameSelectButton _soloButton;
		[SerializeField] private GameSelectButton _duelButton;

		[Inject]
		private void Build()
		{
			Subscribe(_soloButton, OnSoloSelect);
			Subscribe(_duelButton, OnDuelSelect);
		}

		public void Subscribe(GameSelectButton button, UnityEvent unityEvent)
		{
			var eventer = button.GetComponent<EventTrigger>();

			var eventerPointerDown = new EventTrigger.TriggerEvent();
			eventerPointerDown.AddListener((evt) =>
			{
				_soloButton.SetSelect(button == _soloButton);
				_duelButton.SetSelect(button == _duelButton);
			});
			eventer.triggers.Add(new()
			{
				eventID = EventTriggerType.PointerDown,
				callback = eventerPointerDown
			});

			var eventerPointerUp = new EventTrigger.TriggerEvent();
			eventerPointerUp.AddListener((evt) =>
			{
				_soloButton.SetSelect(false);
				_duelButton.SetSelect(false);
			});
			eventer.triggers.Add(new()
			{
				eventID = EventTriggerType.PointerUp,
				callback = eventerPointerUp
			});

			var eventerPointerClick = new EventTrigger.TriggerEvent();
			eventerPointerClick.AddListener((evt) => unityEvent?.Invoke());
			eventer.triggers.Add(new()
			{
				eventID = EventTriggerType.PointerClick,
				callback = eventerPointerClick
			});

		}

		public override void Show()
		{
			base.Show();
			_soloButton.SetSelect(false);
			_duelButton.SetSelect(false);
		}

		public void SoloButtonTouch() => OnSoloSelect?.Invoke();
		public void DuelButtonTouch() => OnDuelSelect?.Invoke();
	}
}
