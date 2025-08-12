using Core.Engine.Components.SaveGame;

namespace Core.Engine.Components.Vibrations
{
	public class VibrationProvider
	: IVibrationProvider
	{
		public bool IsEnabled { get => _platform.IsInitialized; }

		private readonly IVibrationPlatform _platform;
		private VibrationEnable _vibrationEnableSG;

		public VibrationProvider(ISaveGameProvider sessionProvider)
		{

			_vibrationEnableSG = (VibrationEnable)sessionProvider.GetProperty<VibrationEnable>();

#if UNITY_ANDROID && !UNITY_EDITOR
			_platform = new VibrationAndroid();
#elif UNITY_IOS && !UNITY_EDITOR
			_platform = new VibrationIOS();
#else
			_platform = new VibrationEmpty();
#endif
			_platform.Init();

		}

		private bool VibrateReady
		{
			get
			{
				return IsEnabled && _vibrationEnableSG.Value && _platform.HasVibrator();
			}
		}

		public void VibratePop()
		{
			if (!VibrateReady) return;

			_platform.VibratePop();
		}

		public void VibratePeek()
		{
			if (!VibrateReady) return;

			_platform.VibratePeek();
		}

		public void VibrateNope()
		{
			if (!VibrateReady) return;

			_platform.VibrateNope();
		}

		public void Vibrate()
		{
			if (!VibrateReady) return;

			_platform.Vibrate();
		}
	}
}
