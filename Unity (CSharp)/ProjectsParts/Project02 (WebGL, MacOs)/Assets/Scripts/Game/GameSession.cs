using System.Collections.Generic;

namespace Garilla.Games
{
	public class GameSession
	{
		private it.UI.GamePanel _gamePanel;

		public double TimeMyFold;

		public bool WainTableShowCombintaion;
		public bool WainPlayerCombitaions;
		public bool CardTableOut;
		public bool PreflopBetsOut;
		public bool WainWinFlag;
		public double ShowWinCardTableTime;
		public double ShowWinCardPlayerTime;
		public double ShowWinPlayerTime;
		public double CardOnTableTime;
		public List<ulong> CardOutPlayers = new List<ulong>();
		public bool ClearBank;
		public bool IsFullBank;

		private bool _isStraddal = false;
		public bool SettingsStraddal
		{
			get
			{
				return _isStraddal;
			}
			set
			{
				_isStraddal = value;

				TableApi.SkipStraddleOptions(_gamePanel.Table.id, !_isStraddal, null);

			}
		}
		public bool SettingsShowMessage = true;
		public bool SettingsShowEmojies = true;

		public GameSession(it.UI.GamePanel gamePanel){
			_gamePanel = gamePanel;

		}

	}
}