using Engine.Scripts.Managers;
using Game.Scripts.Managers.Base;
using Game.Scripts.UI;
using Game.Scripts.UI.Controllers;
using UnityEngine;

namespace Game.Scripts.Managers
{
	public class PauseHandler : IPauseHandler
	{
		private IUiNavigator _uiHelper;
		private IRhythmDirector _rhythmDirector;
		private GamePausePresenterController _pauseController;

		public bool IsPaused { get; private set; }

		public PauseHandler(IUiNavigator uiHelper, IRhythmDirector rhythmDirector)
		{
			_uiHelper = uiHelper;
			_rhythmDirector = rhythmDirector;
		}

		public void TogglePause()
		{
			if (IsPaused)
			{
				UnPause();
			}
			else
			{
				Pause();
			}
		}

		public void Pause()
		{
			_pauseController ??= _uiHelper.GetController<GamePausePresenterController>();

			IsPaused = true;
			Time.timeScale = 0;
			AudioListener.pause = true;
			_rhythmDirector.Pause();

			_ = _pauseController.Open();
		}

		public void UnPause()
		{
			IsPaused = false;
			Time.timeScale = 1;
			AudioListener.pause = false;
			_rhythmDirector.UnPause();
			_ = _pauseController.Close();
		}
	}
}
