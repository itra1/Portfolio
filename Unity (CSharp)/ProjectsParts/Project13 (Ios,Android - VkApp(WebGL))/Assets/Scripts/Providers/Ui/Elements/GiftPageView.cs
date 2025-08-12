using System.Collections.Generic;
using Game.Providers.TimeBonuses;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class GiftPageView : HomePage
	{
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private List<TimeBonusView> _giftView;

		private ITimeBonusProvider _giftsProvider;

		[Inject]
		public void Constructor(ITimeBonusProvider giftsProvider)
		{
			_giftsProvider = giftsProvider;
		}

		public void Awake()
		{
			ConfirmGifts();
		}

		public void OnEnable()
		{
		}

		private void ConfirmGifts()
		{
			var gifts = _giftsProvider.Gifts;

			for (var i = 0; i < _giftView.Count && i < gifts.Count; i++)
			{
				_giftView[i].SetGift(gifts[i]);
			}
		}
	}
}
