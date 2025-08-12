using Game.Providers.Battles;
using Game.Providers.Battles.Helpers;
using Game.Providers.Profile;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class PlayPageView : HomePage
	{
		private SignalBus _signalBus;
		private IBattleProvider _providerTournament;
		private IBattleHelper _tournamentHelper;
		private IProfileProvider _profileProvider;

		[Inject]
		public void Constructor(SignalBus signalBus,
		IBattleProvider tournamentProvider,
		IBattleHelper tournamentHelper,
		IProfileProvider profileProvider)
		{
			_signalBus = signalBus;
			_providerTournament = tournamentProvider;
			_tournamentHelper = tournamentHelper;
			_profileProvider = profileProvider;
		}

		public void PlayButtonTouch()
		{
			//var tournament = _providerTournament.Items.Find(x => x.Uuid == "duel");

			//_tournamentHelper.RunSolo(tournament, null);
		}
	}
}
