using Game.Scripts.Settings.Common;

namespace Game.Scripts.Controllers.Sessions.Common
{
	public class Session : ISession
	{
		public string SceneVisibleMode { get; private set; }
		public string GameMissMode { get; private set; }
		public bool TapVisible { get; set; } = false;
		public bool IsPlaying { get; set; } = false;

		public bool GameStatistic { get; set; } = false;

		public void SceneVisibleMoveSet(string value) =>
			SceneVisibleMode = value;

		public void SceneVisibleToggle()
		{
			SceneVisibleMode = SceneVisibleMode switch
			{
				SceneVisibleModeType.Orthographic => SceneVisibleModeType.Perspective,
				SceneVisibleModeType.Perspective => SceneVisibleModeType.Orthographic,
				_ => SceneVisibleModeType.Orthographic
			};
		}
		public void GameMissMoveSet(string value) =>
			GameMissMode = value;
		public void GameMissToggle()
		{
			GameMissMode = GameMissMode switch
			{
				GameMissModeType.Loss => GameMissModeType.Rollback,
				GameMissModeType.Rollback => GameMissModeType.Ignore,
				GameMissModeType.Ignore => GameMissModeType.Loss,
				_ => GameMissModeType.Loss
			};
		}

		public void GameStatisticSet(bool value)
			=> GameStatistic = value;
	}
}
