using Game.Providers.Ui.Elements;
using Game.Providers.Ui.Helpers;
using Game.Providers.Ui.Presenters.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Providers.Ui.Windows.Elements
{
	public class GamePlayDuelWindowPresenter : WindowPresenter
	{
		[SerializeField] private GamePlayDuelResultPanel _winPanel;
		[SerializeField] private GamePlayDuelResultPanel _lossPanel;
		[SerializeField] private GamePlayWeaponToolPanel _weaponTool;
		[SerializeField] private ClickListener _screenClickListener;
		[SerializeField] private TMP_Text _timerLabel;
		[SerializeField] private TMP_Text _startTimerLabel;
		[SerializeField] private TMP_Text _roundLabel;
		[SerializeField] private TMP_Text _playerWinLabel;
		[SerializeField] private TMP_Text _botWinLabel;
		[SerializeField] private RawImage _botAvatarImage;
		[SerializeField] private HitIndicationsPanel _playerHitIndicator;
		[SerializeField] private HitIndicationsPanel _botHitIndicator;

		public TMP_Text StartTimerLabel => _startTimerLabel;
		public GamePlayDuelResultPanel WinPanel => _winPanel;
		public GamePlayDuelResultPanel LossPanel => _lossPanel;
		public TMP_Text RoundLabel => _roundLabel;
		public TMP_Text PlayerWinLabel => _playerWinLabel;
		public TMP_Text BotWinLabel => _botWinLabel;
		public HitIndicationsPanel PlayerHitIndicator => _playerHitIndicator;
		public HitIndicationsPanel BotHitIndicator => _botHitIndicator;
		public RawImage BotAvatarImage => _botAvatarImage;
		public ClickListener ScreenClickListener => _screenClickListener;
		public GamePlayWeaponToolPanel WeaponTool => _weaponTool;
		public TMP_Text TimerLabel => _timerLabel;

		public override void Show()
		{
			base.Show();
			_winPanel.gameObject.SetActive(false);
			_lossPanel.gameObject.SetActive(false);
		}
	}
}
