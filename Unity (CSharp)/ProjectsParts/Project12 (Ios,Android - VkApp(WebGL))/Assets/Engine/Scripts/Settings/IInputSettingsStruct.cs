namespace Engine.Scripts.Settings
{
	public interface IInputSettingsStruct
	{
		bool DisableKeyboard { get; }
		bool DisableTouch { get; }
		bool DisableMouse { get; }
		bool SingleTouchSwift { get; }
	}
}
