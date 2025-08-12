using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.Audio;
using Core.Engine.uGUI.Screens;

using UnityEngine.EventSystems;

namespace Core.Engine.uGUI.Screens
{
	[PrefabName(ScreenTypes.BonusGame)]
	public class BonusGameScreen : Screen, IBonusGameScreen, IPointerClickHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			PlayAudio.PlaySound("click");
		}
	}
}