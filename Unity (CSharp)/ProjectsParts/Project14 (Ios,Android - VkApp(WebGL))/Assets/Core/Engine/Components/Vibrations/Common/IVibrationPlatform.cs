namespace Core.Engine.Components.Vibrations
{
	internal interface IVibrationPlatform
	{
		bool IsInitialized { get; set; }
		void Init();

		void VibratePop();
		void VibratePeek();
		void VibrateNope();
		void VibrationCancel();
		bool HasVibrator();
		void Vibrate();
	}
}
