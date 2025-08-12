using Game.Base;
using Game.Game.Settings;
using Game.Providers.Audio.Base;
using Game.Providers.Audio.Handlers;
using Game.Providers.Profile;
using Game.Providers.Profile.Common;
using Game.Providers.Profile.Handlers;
using Game.Providers.Profile.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class PlayerLevelItemView : MonoBehaviour, IPoolable<PlayerLevel, IMemoryPool>
	{
		[SerializeField] private RectTransform _nextFlag;
		[SerializeField] private RectTransform _completeCheck;
		[SerializeField] private RectTransform _completeRewardBack;
		[SerializeField] private RectTransform _completeRewardLight;
		[SerializeField] private Image _indicatorImage;
		[SerializeField] private Image _panelBackImage;
		[SerializeField] private Image _rewardbackImage;
		[SerializeField] private Image _iconeImage;
		[SerializeField] private TMP_Text _levelLabel;
		[SerializeField] private TMP_Text _rewardCountLabel;
		[SerializeField] private Color _blackColor;
		[SerializeField] private Color _blueColor;
		[SerializeField] private Sprite[] _indicatorSprites;
		[SerializeField] private Sprite[] _panelSprites;
		[SerializeField] private Sprite _yellowSprite;
		[SerializeField] private Sprite _greenSprite;
		[SerializeField] private Sprite _blueSprite;

		private SignalBus _signalBus;
		private IProfileProvider _profileProvider;
		private ExperienceHandler _experienceHandler;
		private LevelRewardsHandler _levelRewardsHandler;
		private ResourcesIconsSettings _resourcesItemsSettings;
		private AudioHandler _audioHandler;
		private IMemoryPool _pool;
		private PlayerLevel _playerLevel;

		public int Index => _playerLevel.Index;

		[Inject]
		public void Constructor(SignalBus signalBus,
		IProfileProvider profileProvider,
		ExperienceHandler experienceHandler,
		LevelRewardsHandler levelRewardsHandler,
		ResourcesIconsSettings resourcesItemsSettings,
		AudioHandler audioHandler)
		{
			_signalBus = signalBus;
			_profileProvider = profileProvider;
			_experienceHandler = experienceHandler;
			_levelRewardsHandler = levelRewardsHandler;
			_resourcesItemsSettings = resourcesItemsSettings;
			_audioHandler = audioHandler;
		}

		public void OnEnable()
		{
			_signalBus.Subscribe<ExperienceChangeSignal>(Confirm);
		}

		public void OnDisable()
		{
			_signalBus.Unsubscribe<ExperienceChangeSignal>(Confirm);
		}

		public void OnSpawned(PlayerLevel playerLevel, IMemoryPool pool)
		{
			_pool = pool;
			_playerLevel = playerLevel;

			_levelLabel.text = $"{_playerLevel.Index} level";
			_iconeImage.sprite = _resourcesItemsSettings.Resource.Find(x => x.Name == _playerLevel.LevelRewards.Reward.Type).IconeGroup;
			_rewardCountLabel.text = _playerLevel.LevelRewards.Reward.ValueToString;

			_rewardbackImage.sprite = _playerLevel.LevelRewards.Reward.Type switch
			{
				RewardTypes.Experience => _yellowSprite,
				RewardTypes.Dollar => _greenSprite,
				RewardTypes.Coins => _blueSprite,
				_ => _blueSprite
			};

			Confirm();
		}
		public void OnDespawned()
		{

		}
		public void Despawned()
		{
			_pool.Despawn(this);
		}

		public void MainTouch()
		{

			if (!_playerLevel.RewardReady)
				return;
			_ = _audioHandler.PlayRandomClip(SoundNames.UiTap);
			_levelRewardsHandler.GetRewards(_playerLevel, _iconeImage.transform as RectTransform);
		}

		private void Confirm()
		{

			if (_playerLevel.IsReceivedLevel && _playerLevel.RewardReady)
			{
				StateRewardReady();
			}
			else if (_playerLevel.IsReceivedLevel)
			{
				StateComplete();
			}
			else if (_profileProvider.NextLevel == _playerLevel.Index)
			{
				StateNearest();
			}
			else
			{
				StateFuture();
			}
		}

		[ContextMenu("StateComplete")]
		private void StateComplete()
		{
			_levelLabel.color = _blueColor;
			_nextFlag.gameObject.SetActive(false);
			_completeRewardBack.gameObject.SetActive(false);
			_rewardbackImage.gameObject.SetActive(true);
			_completeCheck.gameObject.SetActive(true);
			_completeRewardLight.gameObject.SetActive(false);
			_indicatorImage.sprite = _indicatorSprites[0];
			_panelBackImage.sprite = _panelSprites[0];
			_levelLabel.color = new Color(0, 0, 0, 0.5f);
			_rewardCountLabel.color = new Color(1, 1, 1, 0.5f);
			_iconeImage.color = new Color(1, 1, 1, 0.5f);
		}

		[ContextMenu("StateRewardReady")]
		private void StateRewardReady()
		{
			_levelLabel.color = _blackColor;
			_nextFlag.gameObject.SetActive(false);
			_completeRewardBack.gameObject.SetActive(true);
			_rewardbackImage.gameObject.SetActive(false);
			_completeCheck.gameObject.SetActive(false);
			_completeRewardLight.gameObject.SetActive(true);
			_indicatorImage.sprite = _indicatorSprites[1];
			_panelBackImage.sprite = _panelSprites[1];
			_levelLabel.color = new Color(0, 0, 0, 1);
			_rewardCountLabel.color = new Color(1, 1, 1, 1);
			_iconeImage.color = new Color(1, 1, 1, 1f);
		}

		[ContextMenu("StateNearest")]
		private void StateNearest()
		{
			_nextFlag.gameObject.SetActive(true);
			StateNext();
		}

		[ContextMenu("StateFuture")]
		private void StateFuture()
		{
			_nextFlag.gameObject.SetActive(false);
			StateNext();
		}

		private void StateNext()
		{
			_levelLabel.color = _blueColor;
			_completeRewardBack.gameObject.SetActive(false);
			_rewardbackImage.gameObject.SetActive(true);
			_completeCheck.gameObject.SetActive(false);
			_completeRewardLight.gameObject.SetActive(false);
			_indicatorImage.sprite = _indicatorSprites[2];
			_panelBackImage.sprite = _panelSprites[2];
			_levelLabel.color = new Color(0, 0, 0, 1);
			_rewardCountLabel.color = new Color(1, 1, 1, 1);
			_iconeImage.color = new Color(1, 1, 1, 1f);
		}

		public class Factory : PlaceholderFactory<PlayerLevel, PlayerLevelItemView> { }
	}
}
