using Engine.Scripts.Base;
using StringDrop;
using UnityEngine;

namespace Game.Scripts.UI.Settings
{
	[System.Serializable]
	public class ProductButtonColorsUiItem
	{
		[SerializeField][StringDropList(typeof(ColorTypes))] private string _type;
		[SerializeField] private Color _color;
		[SerializeField] private Sprite _iconeBack;

		public string Type => _type;
		public Color Color => _color;
		public Sprite IconeBack => _iconeBack;
	}
}
