using UnityEngine;

namespace Engine.Scripts.Common.Interfaces
{
	public interface ISceneUiParents
	{
		RectTransform WindowsParent { get; }
		RectTransform PopupParent { get; }
		RectTransform SplashParent { get; }
	}
}
