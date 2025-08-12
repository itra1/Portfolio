namespace Game.Scripts.Controllers.Sessions.Debugs
{
	public interface IDebugSession
	{
		/// <summary>
		/// Как реагировать на пропуск ноты
		/// </summary>
		string GameMissMode { get; }
		/// <summary>
		/// Активация статистики в игровом режиме
		/// </summary>
		bool GameStatistic { get; }
		/// <summary>
		/// Отображение тапов на экране
		/// </summary>
		bool TapVisible { get; set; }

		void GameMissMoveSet(string value);
		void GameMissToggle();
		void GameStatisticSet(bool value);
	}
}
