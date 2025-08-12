using Core.Engine.uGUI.Popups;
using Core.Engine.uGUI.Screens;
using UnityEngine;

namespace Core.Engine.Components.Game
{
	public class SceneComponentsBase :MonoBehaviour, ISceneComponent, IScreensParent, IPopupsParent
	{
		[SerializeField] protected RectTransform _screenParent;
		[SerializeField] protected RectTransform _popupParent;
		public RectTransform ScreenParent => _screenParent;
		public RectTransform PopupParent => _popupParent;
	}
}
