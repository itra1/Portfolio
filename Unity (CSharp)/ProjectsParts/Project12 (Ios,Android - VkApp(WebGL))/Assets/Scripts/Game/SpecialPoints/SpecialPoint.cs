using StringDrop;
using UnityEngine;

namespace Game.Game.SpecialPoints {
	public class SpecialPoint : MonoBehaviour {
		[SerializeField, StringDropList(typeof(SpecialPointNames))] private string _name;
		[SerializeField] private Sprite _activeSprite;
		[SerializeField] private Sprite _backSprite;
		[SerializeField] private Sprite _graySprite;

		public string Name => _name;
		public Sprite ActiveSprite => _activeSprite;
		public Sprite BackSprite => _backSprite;
		public Sprite GraySprite => _graySprite;
	}
}
