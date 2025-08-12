using Game.Base;
using Game.Game.Settings;
using Game.Providers.Battles.Helpers;
using Game.Providers.Battles.Settings;
using Game.Providers.Profile.Handlers;
using Game.Providers.Profile.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class TournamentView : MonoBehaviour, IPoolable<Battles.Settings.DuelItemSettings, IMemoryPool>
	{

		public UnityAction<DuelItemSettings> OnClick;

		[SerializeField] private TMP_Text _titleLabel;
		[SerializeField] private TMP_Text _winLabel;
		[SerializeField] private TMP_Text _winTypeLabel;
		[SerializeField] private TMP_Text _feeLabel;
		[SerializeField] private TMP_Text _playersLabel;
		[SerializeField] private TMP_Text _playButtonTitle;
		[SerializeField] private Image _winIconeBack;
		[SerializeField] private Image _winIconeLabel;
		[SerializeField] private Image _feeIcone;
		[SerializeField] private Button _playButton;
		[SerializeField] private TimerView _timer;
		[SerializeField] private Sprite _winBackGreen;
		[SerializeField] private Sprite _winBackYellow;
		[SerializeField] private Button _playButtonPlay;
		[SerializeField] private Sprite _playButtonEnableSprite;
		[SerializeField] private Sprite _playButtonDisableSprite;
		[SerializeField] private Material _playButtonTitleRedMaterial;
		[SerializeField] private Material _playButtonTitleGrayMaterial;

		private IMemoryPool _pool;
		private DuelItemSettings _tournamentItem;
		private SignalBus _signalBus;
		private IBattleHelper _battleHelper;
		private ResourcesIconsSettings _resourcesIconsSettings;
		private PlayerResourcesHandler _resourcesHandler;

		public DuelItemSettings TournamentItem { get => _tournamentItem; set => _tournamentItem = value; }
		public Button PlayButton { get => _playButton; set => _playButton = value; }

		[Inject]
		public void Constructor(SignalBus signalbus, IBattleHelper battleHelper, ResourcesIconsSettings resourcesIconsSettings, PlayerResourcesHandler resourcesHandler)
		{
			_signalBus = signalbus;
			_battleHelper = battleHelper;
			_resourcesIconsSettings = resourcesIconsSettings;
			_resourcesHandler = resourcesHandler;
		}

		public void Awake()
		{
			_playButton.onClick.AddListener(PlayGame);
		}

		public void OnEnable()
		{
			_signalBus.Subscribe<MoneyChangeSignal>(OnMoneyChangeSignal);
		}

		public void OnDisable()
		{
			_signalBus.Unsubscribe<MoneyChangeSignal>(OnMoneyChangeSignal);
		}

		public void OnDespawned()
		{
		}

		public void OnSpawned(Battles.Settings.DuelItemSettings tournament, IMemoryPool pool)
		{
			_pool = pool;
			_tournamentItem = tournament;
			SetTournament();
		}

		public void Despawned()
		{
			_pool.Despawn(this);
		}

		private void OnMoneyChangeSignal()
		{
			SetTournament();
		}

		public void SetTournament()
		{
			var rewardType = _tournamentItem.Reward;
			var feeType = _tournamentItem.Fee;

			var reward = _resourcesIconsSettings.Resource.Find(x => x.Name == rewardType.Type);
			var fee = _resourcesIconsSettings.Resource.Find(x => x.Name == feeType.Type);

			_winIconeBack.sprite = rewardType.Type switch
			{
				RewardTypes.Dollar => _winBackGreen,
				_ => _winBackYellow
			};
			SetButtonState(_resourcesHandler.GetHandler(feeType.Type).CurrentValue >= feeType.Value);

			_feeIcone.sprite = fee.IconeMini;
			_titleLabel.text = _tournamentItem.Title;
			_feeLabel.text = feeType.ValueToString;
			_winLabel.text = rewardType.ValueToString;
			_winIconeLabel.sprite = reward.IconeBig;
			_winTypeLabel.text = reward.VisibleName;
			_playersLabel.text = $"{_tournamentItem.PlayersCount}";
			_timer.gameObject.SetActive(false);
		}

		private void SetButtonState(bool isReady)
		{
			_playButtonTitle.text = isReady ? "Play" : "Not enough money";
			_playButtonTitle.fontMaterial = isReady ? _playButtonTitleRedMaterial : _playButtonTitleGrayMaterial;
			_playButtonPlay.interactable = isReady;
			_playButtonPlay.GetComponent<Image>().sprite = isReady ? _playButtonEnableSprite : _playButtonDisableSprite;
		}

		public void PlayGame()
		{
			OnClick?.Invoke(_tournamentItem);
		}

		public class Factory : PlaceholderFactory<Battles.Settings.DuelItemSettings, TournamentView> { }
	}
}
