using Game.Base;
using Game.Game.Settings;
using Game.Providers.Audio.Base;
using Game.Providers.Audio.Handlers;
using Game.Providers.DailyBonus;
using Game.Providers.DailyBonus.Elements;
using Game.Providers.DailyBonus.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class DailyBonusView : MonoBehaviour, IInjection
	{

		public UnityAction OnGet;

		[SerializeField] private RectTransform _nextBack;
		[SerializeField] private RectTransform _completeBack;
		[SerializeField] private RectTransform _activeBack;
		[SerializeField] private TMP_Text _titleLabel;
		[SerializeField] private Image _ckeckIcone;
		[SerializeField] private Color _grayColor;
		[SerializeField] private Material _completeTitleMaterial;
		[SerializeField] private Material _nextTitleMaterial;
		[SerializeField] private Reward[] _rewards;

		private ResourcesIconsSettings _resourcesIcons;
		private DailyBonusHandler _dailyBonusHandler;
		private DailyBonusItem _dailyBonus;
		private AudioHandler _audioHandler;
		private SignalBus _signalBus;

		public bool IsGetReady => _dailyBonus.IsGetReady;

		[Inject]
		public void Constructor(SignalBus signalBus, ResourcesIconsSettings resourcesIcons, DailyBonusHandler dailyBonusHandler, AudioHandler audioHandler)
		{
			_signalBus = signalBus;
			_resourcesIcons = resourcesIcons;
			_dailyBonusHandler = dailyBonusHandler;
			_audioHandler = audioHandler;
			_signalBus.Subscribe<DailyBonusChangeSignal>(Confirm);

		}

		public void Awake()
		{
			GetComponent<Button>().onClick.AddListener(MainButtonTouch);
		}

		public void MainButtonTouch()
		{
			if (!_dailyBonus.IsGetReady)
				return;
			_ = _audioHandler.PlayRandomClip(SoundNames.UiTap);
			GetReward();
		}

		public void GetReward()
		{
			_dailyBonusHandler.GetBonus(_dailyBonus, transform as RectTransform);
			OnGet?.Invoke();
		}

		public void SetData(DailyBonusItem dailyBonusItem)
		{
			_dailyBonus = dailyBonusItem;
			Confirm();
		}

		private void Confirm()
		{
			if (_dailyBonus == null)
				return;
			var isNext = !_dailyBonus.IsGet && !_dailyBonus.IsGetReady;
			_titleLabel.color = isNext ? Color.white : Color.black;
			_titleLabel.text = _dailyBonus.IsGetReady ? "Today" : $"Day {_dailyBonus.Index + 1}";
			_titleLabel.fontMaterial = isNext ? _nextTitleMaterial : _completeTitleMaterial;
			_nextBack.gameObject.SetActive(isNext);
			_completeBack.gameObject.SetActive(_dailyBonus.IsGet);
			_activeBack.gameObject.SetActive(_dailyBonus.IsGetReady);
			_ckeckIcone.gameObject.SetActive(_dailyBonus.IsGet);

			for (var i = 0; i < _rewards.Length && i < _dailyBonus.Setting.Rewards.Length; i++)
			{
				var res = _resourcesIcons.Resource.Find(x => x.Name == _dailyBonus.Setting.Rewards[i].Reward);
				_rewards[i].Icone.color = new(1, 1, 1, _dailyBonus.IsGet ? 0.5f : 1);
				_rewards[i].Icone.sprite = res.IconeGroup;
				_rewards[i].CountLabel.color = new(1, 1, 1, _dailyBonus.IsGet ? 0.5f : 1);
				_rewards[i].CountLabel.text = _dailyBonus.Setting.Rewards[i].ValueToString;
			}
		}

		[System.Serializable]
		public class Reward
		{
			public Image Icone;
			public TMP_Text CountLabel;
		}

	}
}
