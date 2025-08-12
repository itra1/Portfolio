using Game.Base;
using Game.Game.Settings;
using Game.Providers.TimeBonuses.Base;
using Game.Providers.TimeBonuses.Handlers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class TimeBonusView : MonoBehaviour, IPoolable<ITimeBonus, IMemoryPool>, IInjection
	{
		[SerializeField] private TimerView _timer;
		[SerializeField] private TMP_Text _countLabel;
		[SerializeField] private TMP_Text _rewardTypeLabel;
		[SerializeField] private Button _claimButton;
		[SerializeField] private Image _iconeImage;
		[SerializeField] private GameObject _animationLight;

		private ResourcesIconsSettings _resourcesIconsSettings;
		private TimeBonusHandler _timeBonusHandler;
		private IMemoryPool _pool;
		private ITimeBonus _gift;

		[Inject]
		public void Constructor(ResourcesIconsSettings resourcesIconsSettings, TimeBonusHandler timeBonusHandler)
		{
			_resourcesIconsSettings = resourcesIconsSettings;
			_timeBonusHandler = timeBonusHandler;
		}

		public void OnDespawned() { }
		public void Despawned()
		{
			_pool.Despawn(this);
		}

		public void OnSpawned(ITimeBonus gift, IMemoryPool pool)
		{
			_pool = pool;
			SetGift(gift);
		}

		public void SetGift(ITimeBonus gift)
		{
			_gift = gift;
			ConfirmData();
			_gift.OnChange.AddListener(() =>
			{
				ConfirmData();
			});
		}

		private void ConfirmData()
		{
			var res = _resourcesIconsSettings.Resource.Find(x => x.Name == _gift.RewardType);
			_iconeImage.sprite = res.IconeGroup;
			_rewardTypeLabel.text = res.VisibleName;
			_countLabel.text = _gift.Value.ToString();
			_timer.gameObject.SetActive(!_gift.GetReady);
			_animationLight.SetActive(_gift.GetReady);
			_claimButton.gameObject.SetActive(_gift.GetReady);
			if (!_gift.GetReady)
			{
				_ = _gift.TimerToReady.OnComplete(() =>
				{
					ConfirmData();
				});
				_ = _gift.TimerToReady.OnStart(() =>
				{
					ConfirmData();
				});
				_timer.SetTimer(_gift.TimerToReady);
			}
		}

		public void ClaimButtonTouch()
		{
			_timeBonusHandler.Confirm(_gift, transform as RectTransform);
		}

		public class Factory : PlaceholderFactory<ITimeBonus, TimeBonusView> { }
	}
}
