#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
using Platforms.RateUs.Settings;
using Platforms.Settings;
using UnityEngine;
using UnityEngine.Events;
/// Смотреть документацию плагина https://github.com/nicloay/appirater-unity/tree/master?tab=readme-ov-file

namespace Platforms.RateUs.Adapters
{
	public class TDAppleAdapter : ITDRateUsAdapter
	{
		private readonly TDPlatformSettings _platformSettings;
		private readonly TDRateUsSettings _settings;

		public bool ReadyShow => true;

		public TDAppleAdapter()
		{
			_platformSettings = Resources.Load<TDPlatformSettings>($"Settings/{TDPlatformSettings.FileName}");
			_settings = Resources.Load<TDRateUsSettings>($"Settings/{TDRateUsSettings.FileName}");
		}

		public void RateUs(UnityAction<bool> completeCallback)
		{
			//DidSignificantEvent();
			Init(_platformSettings.IosSettings.AppId,
			_settings.Ios.DaysUntilPrompt,
			_settings.Ios.UsesUntilPrompt,
			_settings.Ios.SignificantEventsUntilPrompt,
			_settings.Ios.TimeBeforeReminding,
			_settings.Ios.IsDebug
			);
			completeCallback?.Invoke(true);
		}

		public static void Init(string appId, int daysUntilPrompt, int usesUntilPrompt,
			int significantEventsUntilPrompt, int timeBeforeReminding, bool debug)
		{
			_Init(appId, daysUntilPrompt, usesUntilPrompt, significantEventsUntilPrompt, timeBeforeReminding, debug);
		}

		public static void DidSignificantEvent()
		{
			_DidSignificantEvent();
		}

		public void AppGoesToBackground(bool isInBackground)
		{
			AppGoesToBackground(isInBackground);
		}

		[DllImport("__Internal")]
		private static extern void _Init(string appId, int daysUntilPrompt, int usesUntilPrompt,
			int significantEventsUntilPrompt, int timeBeforeReminding, bool debug);

		[DllImport("__Internal")]
		private static extern void _AppGoesToBackground(bool idInBackground);

		[DllImport("__Internal")]
		private static extern void _DidSignificantEvent();
	}
}
#endif