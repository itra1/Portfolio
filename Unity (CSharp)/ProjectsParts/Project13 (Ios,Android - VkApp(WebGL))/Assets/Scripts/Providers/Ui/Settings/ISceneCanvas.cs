using UnityEngine;

namespace Game.Providers.Ui.Settings
{
	public interface ISceneCanvas
	{
		RectTransform PopupsParent { get; }
		RectTransform WindowsParent { get; }
		RectTransform SplashParent { get; }
	}
}
