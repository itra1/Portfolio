using Core.Engine.Components.SaveGame;
using UnityEngine;
using Zenject;

namespace Game.Tools
{
	public class LevelResetButton :MonoBehaviour
	{
		[Inject]
		public SaveGameProvider _saveGameProvider;
		private GameLevelSG _gameLevel;

		public void ResetLevelTouch()
		{
			if (_gameLevel == null)
				_gameLevel = (GameLevelSG)_saveGameProvider.GetProperty<GameLevelSG>();

			_gameLevel.Value = 0;
			_gameLevel.Save();
		}
	}
}
