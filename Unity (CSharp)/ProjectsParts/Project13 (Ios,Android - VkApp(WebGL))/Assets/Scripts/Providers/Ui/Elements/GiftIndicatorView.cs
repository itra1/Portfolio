using Game.Base;
using Game.Providers.TimeBonuses;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class GiftIndicatorView : MonoBehaviour, IInjection
	{
		[SerializeField] private RectTransform _body;
		[SerializeField] private TMP_Text _countLabel;

		private SignalBus _signalbus;
		private ITimeBonusProvider _timeBonusProvider;

		[Inject]
		public void Constructor(SignalBus signalBus, ITimeBonusProvider timebonusProvider)
		{
			_signalbus = signalBus;
			_timeBonusProvider = timebonusProvider;
		}

		public void OnEnable()
		{
			_timeBonusProvider.OnTimeBonusChangeEvent.AddListener(ConfirmCount);
			ConfirmCount();
		}

		public void OnDisable()
		{
			_timeBonusProvider.OnTimeBonusChangeEvent.RemoveListener(ConfirmCount);
		}

		private void ConfirmCount()
		{
			_body.gameObject.SetActive(_timeBonusProvider.CountReady > 0);
			_countLabel.text = _timeBonusProvider.CountReady.ToString();
		}
	}
}
