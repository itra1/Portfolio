using Game.Base;
using Game.Game.Handlers;
using Game.Providers.Profile;
using Game.Providers.Profile.Handlers;
using Game.Providers.Profile.Signals;
using Game.Providers.Ui.Presenters.Interfaces;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class CoinsPanel : MonoBehaviour, IInjection, IUiVisibleHandler
	{
		[SerializeField] private TMP_Text _coinsLabel;

		private SignalBus _signalBus;
		private IProfileProvider _profileProvider;
		private CoinsHandler _coinsHandler;
		private OpenAddCoinsHandler _openAddCoinsHandler;

		[Inject]
		public void Constructor(SignalBus signalBus, IProfileProvider profileProvider, CoinsHandler coinsHandler, OpenAddCoinsHandler openAddCoinsHandler)
		{
			_signalBus = signalBus;
			_profileProvider = profileProvider;
			_coinsHandler = coinsHandler;
			_openAddCoinsHandler = openAddCoinsHandler;
		}

		public void Show()
		{
			_signalBus.Subscribe<CoinsChangeSignal>(OnCoinsChangeSignal);
			OnCoinsChangeSignal();
		}

		public void Hide()
		{
			_signalBus.Unsubscribe<CoinsChangeSignal>(OnCoinsChangeSignal);
		}

		private void OnCoinsChangeSignal()
		{
			_coinsLabel.text = $"<sprite=0>{_coinsHandler.CurrentValueString}";
		}

		private void AddButtonTouch()
		{
			_openAddCoinsHandler.OpenAddCoinsDialog();
		}
	}
}
