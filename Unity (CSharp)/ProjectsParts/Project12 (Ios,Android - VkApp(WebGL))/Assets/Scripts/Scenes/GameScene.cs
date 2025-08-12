using Engine.Scripts.Scenes;
using UnityEngine;

namespace Game.Scripts.Scenes
{
	public class GameScene : SceneBase
	{
		private void Awake()
		{
			ResizeBackground();
		}

		[ContextMenu("ResizeBackground")]
		private void ResizeBackground()
		{
			//BackGround.FillScreenPerspectiveCamera();
		}
	}
}
