using UnityEngine;
using Sett = it.Settings;

namespace it.Popups
{
	public class ColorLabelsPopup : PopupBase
	{
		[SerializeField] private ColorLabelsItem[] _items;

		private void OnEnable()
		{
			for(int i = 0; i < _items.Length; i++){
				_items[i].Label.color = Sett.AppSettings.Sticks.Sticks[i].Color;
			}
		}

		public void SaveTouch()
		{

		}
	}
}