using Engine.Scripts.Common.Interfaces;
using UnityEngine;

namespace Game.Scripts.Helpers
{
	public class SceneModeHelper
	{
		private readonly ISceneTracks _worldTracks;
		private readonly ISceneCamera _sceneCamera;

		public SceneModeHelper(ISceneTracks worldTracks, ISceneCamera sceneCamera)
		{
			_worldTracks = worldTracks;
			_sceneCamera = sceneCamera;
		}

		public void SetMode(bool isOrthographic)
		{
			if (isOrthographic)
				OrtograficMode();
			else
				PerspectiveMode();
		}

		private void PerspectiveMode()
		{
			_sceneCamera.Camera.orthographic = false;
			_worldTracks.TracksParent.rotation = Quaternion.Euler(new(30, 0, 0));
		}

		private void OrtograficMode()
		{
			_sceneCamera.Camera.orthographic = true;
			_worldTracks.TracksParent.rotation = Quaternion.Euler(new(0, 0, 0));
		}
	}
}
