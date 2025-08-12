using Game.Base;
using Game.Providers.Battles;
using Game.Providers.Battles.Signals;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class TournamentRewardNotificationView : MonoBehaviour, IInjection
	{
		[SerializeField] private RectTransform _icone;

		private SignalBus _signalBus;
		private IBattleProvider _tournamentProvider;

		[Inject]
		public void Constructor(SignalBus signalBus, IBattleProvider tournamentProvider)
		{
			_signalBus = signalBus;
			_tournamentProvider = tournamentProvider;
		}

		public void OnEnable()
		{
			_signalBus.Subscribe<BattleResultChangeSignal>(ConfirmState);
			ConfirmState();
		}

		public void OnDisable()
		{
			_signalBus.Unsubscribe<BattleResultChangeSignal>(ConfirmState);
		}

		private void ConfirmState()
		{
			_icone.gameObject.SetActive(_tournamentProvider.ExistsRewards);
		}
	}
}
