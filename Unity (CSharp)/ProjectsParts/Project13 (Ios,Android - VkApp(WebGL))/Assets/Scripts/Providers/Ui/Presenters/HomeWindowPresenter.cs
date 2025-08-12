using Cysharp.Threading.Tasks;
using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Elements;
using Game.Providers.Ui.Presenters.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Providers.Ui.Presenters
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.Home)]
	public class HomeWindowPresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent OnPlayButton = new();
		[HideInInspector] public UnityEvent OnShopButton = new();
		[HideInInspector] public UnityEvent OnLeadersButton = new();
		[HideInInspector] public UnityEvent OnDailyMission = new();
		[HideInInspector] public UnityEvent OnDailyBonus = new();
		[HideInInspector] public UnityEvent OnWatchAds = new();
		[HideInInspector] public UnityEvent OnBattle = new();

		[SerializeField] private Button _playButton;
		[SerializeField] private Button _shopButton;
		[SerializeField] private Button _leadersButton;
		[SerializeField] private Button _dailyMissionButton;
		[SerializeField] private Button _dailyBonusButton;
		[SerializeField] private Button _watchAdsButton;
		[SerializeField] private Button _battleButton;

		public TournamentPageView TournamentPageView => null;

		public override async UniTask<bool> Initialize()
		{
			if (!await base.Initialize())
				return false;

			_playButton.onClick.AddListener(PlayButtonTouch);
			_shopButton.onClick.AddListener(ShopButtonTouch);
			_leadersButton.onClick.AddListener(LeaderButtonTouch);
			_dailyMissionButton.onClick.AddListener(DailyMissionTouch);
			_dailyBonusButton.onClick.AddListener(DailyBonusTouch);
			_watchAdsButton.onClick.AddListener(WatchAdsTouch);
			_battleButton.onClick.AddListener(BattleTouch);

			return true;
		}

		private void BattleTouch() => OnBattle?.Invoke();
		private void WatchAdsTouch() => OnWatchAds?.Invoke();
		private void DailyBonusTouch() => OnDailyBonus?.Invoke();
		private void DailyMissionTouch() => OnDailyMission?.Invoke();
		private void PlayButtonTouch() => OnPlayButton?.Invoke();
		private void ShopButtonTouch() => OnShopButton?.Invoke();
		private void LeaderButtonTouch() => OnLeadersButton?.Invoke();
	}
}
