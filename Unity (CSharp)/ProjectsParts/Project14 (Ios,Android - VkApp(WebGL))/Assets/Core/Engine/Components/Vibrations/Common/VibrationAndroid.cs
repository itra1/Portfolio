using UnityEngine;

namespace Core.Engine.Components.Vibrations
{
	#if UNITY_ANDROID
	internal class VibrationAndroid : IVibrationPlatform
	{
		public static AndroidJavaClass unityPlayer;
		public static AndroidJavaObject currentActivity;
		public static AndroidJavaObject vibrator;
		public static AndroidJavaObject context;

		public static AndroidJavaClass vibrationEffect;

		public bool IsInitialized { get; set; } = false;

		public static int AndroidVersion
		{
			get
			{
				int iVersionNumber = 0;
				if (Application.platform == RuntimePlatform.Android)
				{
					string androidVersion = SystemInfo.operatingSystem;
					int sdkPos = androidVersion.IndexOf("API-");
					iVersionNumber = int.Parse(androidVersion.Substring(sdkPos + 4, 2).ToString());
				}
				return iVersionNumber;
			}
		}

		public void Init()
		{
			if (IsInitialized) return;


			if (Application.isMobilePlatform)
			{

				unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
				context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

				if (AndroidVersion >= 26)
				{
					vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect");
				}

			}

			IsInitialized = true;
		}

		public void VibratePop()
		{
			VibrateAndroid(50);
		}

		public void VibratePeek()
		{
			VibrateAndroid(100);
		}

		public void VibrateNope()
		{
			long[] pattern = { 0, 50, 50, 50 };
			VibrateAndroid(pattern, -1);
		}

		public void VibrateAndroid(long milliseconds)
		{

			if (Application.isMobilePlatform)
			{
				if (AndroidVersion >= 26)
				{
					AndroidJavaObject createOneShot = vibrationEffect.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, -1);
					vibrator.Call("vibrate", createOneShot);

				}
				else
				{
					vibrator.Call("vibrate", milliseconds);
				}
			}
		}

		public void VibrateAndroid(long[] pattern, int repeat)
		{
			if (Application.isMobilePlatform)
			{
				if (AndroidVersion >= 26)
				{
					long[] amplitudes;
					AndroidJavaObject createWaveform = vibrationEffect.CallStatic<AndroidJavaObject>("createWaveform", pattern, repeat);
					vibrator.Call("vibrate", createWaveform);

				}
				else
				{
					vibrator.Call("vibrate", pattern, repeat);
				}
			}
		}

		public void VibrationCancel()
		{
			if (Application.isMobilePlatform)
			{
				vibrator.Call("cancel");
			}
		}

		public bool HasVibrator()
		{
			if (Application.isMobilePlatform)
			{
				AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context");
				string Context_VIBRATOR_SERVICE = contextClass.GetStatic<string>("VIBRATOR_SERVICE");
				AndroidJavaObject systemService = context.Call<AndroidJavaObject>("getSystemService", Context_VIBRATOR_SERVICE);
				if (systemService.Call<bool>("hasVibrator"))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public void Vibrate()
		{
			if (Application.isMobilePlatform)
			{
				Handheld.Vibrate();
			}
		}

	}
	#endif
}