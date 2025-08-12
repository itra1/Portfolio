using UnityEngine;
using UnityEngine.UI;

namespace Game.Providers.Ui.Elements {
	public class TournamentPageView :HomePage {
		[SerializeField] private Sprite _activeButtonBack;
		[SerializeField] private Sprite _disactiveButtonBack;
		[SerializeField] private Image _tournamentButtonImage;
		[SerializeField] private Image _tournamentResultButtonImage;
		[SerializeField] private TournamentsListView _tournamentListView;
		[SerializeField] private TournamentResultListView _tournamentResultListView;

		public TournamentsListView TournamentsListView => _tournamentListView;

		public void OnEnable() {
			SetPage(PageType.Tournament);
		}

		public void TournamentPageButtonTouch() {
			SetPage(PageType.Tournament);
		}

		public void TournamentResultsPageButtonTouch() {
			SetPage(PageType.TournamentResult);
		}

		public void SetPage(string type) {
			_tournamentButtonImage.sprite = PageType.Tournament == type ? _activeButtonBack : _disactiveButtonBack;
			_tournamentResultButtonImage.sprite = PageType.TournamentResult == type ? _activeButtonBack : _disactiveButtonBack;
			_tournamentListView.gameObject.SetActive(PageType.Tournament == type);
			_tournamentResultListView.gameObject.SetActive(PageType.TournamentResult == type);
		}

		public struct PageType {
			public const string Tournament = "Tournament";
			public const string TournamentResult = "TournamentResult";
		}

	}
}
