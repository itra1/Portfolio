using Engine.Scripts.Settings;
using UnityEngine;

namespace Game.Scripts.Settings
{
	[System.Serializable]
	public class InputSettingsStruct : IInputSettingsStruct
	{
		[SerializeField] private bool _singleTouchSwift;
		[SerializeField] private bool _disableKeyboard;
		[SerializeField] private bool _disableTouch;
		[SerializeField] private bool _disableMouse;

		public bool DisableKeyboard => _disableKeyboard;
		public bool DisableTouch => _disableTouch;
		public bool DisableMouse => _disableMouse;
		public bool SingleTouchSwift => _singleTouchSwift;
	}
}
