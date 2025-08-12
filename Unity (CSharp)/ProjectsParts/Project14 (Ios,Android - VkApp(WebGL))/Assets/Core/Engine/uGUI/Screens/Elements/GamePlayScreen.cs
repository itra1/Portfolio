using Core.Engine.App.Base;
using Core.Engine.App.Base.Attributes;
using Core.Engine.App.Common;
using Core.Engine.Components.Audio;
using Core.Engine.Components.GameQuests;
using Core.Engine.Signals;
using Core.Engine.uGUI.Elements;
using Core.Engine.uGUI.Popups;
using TMPro;
using UnityEngine;
using Zenject;

namespace Core.Engine.uGUI.Screens
{
	[PrefabName(ScreenTypes.GamePlay)]
	public class GamePlayScreen :Screen, IGamePlayScreen
	{
		[SerializeField] private TextMeshProUGUI _scopeLabel;
		[SerializeField] private GameObject _allDestroyQuest;
		[SerializeField] private GameDecPointPanel _getPointsQuest;
		[SerializeField] private GameDecTimerPanel _liveTimeQuest;

		private float _time;
		private GamePointChangeSignal _onGamePointsChange;
		private IAppController _appcontroller;
		private IPopupProvider _popupProvider;
		private IGameQuestProvider _questProvider;

		[Inject]
		public void Initialize(IAppController appController
		, IPopupProvider popupProvider
		, IGameQuestProvider questProvider)
		{
			_appcontroller = appController;
			_popupProvider = popupProvider;
			_questProvider = questProvider;

			_signalBus.Subscribe<GamePointChangeSignal>(OnPointsSignal);
			_signalBus.Subscribe<GameStateChangeSignal>(GameStateChange);
			_signalBus.Subscribe<ActiveQuestSetSignal>(OnActiveQuestSetSignal);
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<GamePointChangeSignal>(OnPointsSignal);
			_signalBus.Unsubscribe<GameStateChangeSignal>(GameStateChange);
			_signalBus.Unsubscribe<ActiveQuestSetSignal>(OnActiveQuestSetSignal);
		}

		private void OnEnable()
		{
			FillPoint(0);
			SetActiveQuest(_questProvider.ActiveQuest);
		}

		private void OnActiveQuestSetSignal(ActiveQuestSetSignal signal)
		{
			SetActiveQuest(signal.Quest);
		}

		private void SetActiveQuest(GameQuest quest)
		{
			if (quest is TimeLiveQuest tl)
			{
				_allDestroyQuest.SetActive(false);
				_getPointsQuest.gameObject.SetActive(false);
				_liveTimeQuest.gameObject.SetActive(true);
				_liveTimeQuest.StartTimer(tl);
			}
			if (quest is AllItemQuest)
			{
				_allDestroyQuest.SetActive(true);
				_getPointsQuest.gameObject.SetActive(false);
				_liveTimeQuest.gameObject.SetActive(false);
			}
			if (quest is GetPointsQuest pt)
			{
				_allDestroyQuest.SetActive(false);
				_getPointsQuest.gameObject.SetActive(true);
				_getPointsQuest.StartValue(pt.TargetPoints);
				_liveTimeQuest.gameObject.SetActive(false);
			}
		}

		private void GameStateChange()
		{
			StartGame();
		}

		private void GamePointChange()
		{
			FillPoint(0);
		}

		private void StartGame()
		{
			if (_appcontroller.AppState != AppGameState.Game)
				return;

			FillPoint(0);
		}

		private void OnPointsSignal(GamePointChangeSignal pp)
		{
			FillPoint(pp.Value);
		}
		private void FillPoint(float pointValue)
		{
			//if (_gameSessionPoints == null) return;
			//_scopeLabel.text = $"Score <color=#FFD600><b>{pointValue}";
			if (_scopeLabel != null)
				_scopeLabel.text = $"{pointValue}";
		}

		/// <summary>
		/// Меню настроек игрового процесса
		/// </summary>
		public void GameMenuButtonTouch()
		{
			PlayAudio.PlaySound("click");
			bool popup = _popupProvider.OpenPopup(PopupTypes.GameMenu);
		}
	}
}