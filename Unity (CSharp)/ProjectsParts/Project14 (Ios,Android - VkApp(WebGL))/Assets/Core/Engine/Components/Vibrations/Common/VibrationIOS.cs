using System.Collections;
using System.Runtime.InteropServices;

using UnityEngine;

namespace Core.Engine.Components.Vibrations
{
#if UNITY_IOS
	internal class VibrationIOS : IVibrationPlatform
	{

		[DllImport("__Internal")]
		private static extern bool _HasVibrator();

		[DllImport("__Internal")]
		private static extern void _Vibrate();

		[DllImport("__Internal")]
		private static extern void _VibratePop();

		[DllImport("__Internal")]
		private static extern void _VibratePeek();

		[DllImport("__Internal")]
		private static extern void _VibrateNope();

		[DllImport("__Internal")]
		private static extern void _impactOccurred(string style);

		[DllImport("__Internal")]
		private static extern void _notificationOccurred(string style);

		[DllImport("__Internal")]
		private static extern void _selectionChanged();

		public bool IsInitialized { get; set; } = false;

		public void Init()
		{
			if (IsInitialized) return;


			IsInitialized = true;

		}

		public void VibratePop()
		{
			_VibratePop();
		}

		public void VibratePeek()
		{
			_VibratePeek();
		}

		public void VibrateNope()
		{
			_VibrateNope();
		}



		public static void VibrateIOS(ImpactFeedbackStyle style)
		{
        _impactOccurred(style.ToString());
		}

		public static void VibrateIOS(NotificationFeedbackStyle style)
		{
        _notificationOccurred(style.ToString());
		}

		public static void VibrateIOS_SelectionChanged()

		{
        _selectionChanged();
		}

		public void CancelAndroid()
		{
			if (Application.isMobilePlatform)
			{
			}
		}

		public bool HasVibrator()
		{
			if (Application.isMobilePlatform)
			{
        return _HasVibrator ();
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

		public void VibrationCancel()
		{
		}
	}

	public enum ImpactFeedbackStyle
	{
		Heavy,
		Medium,
		Light,
		Rigid,
		Soft
	}

	public enum NotificationFeedbackStyle
	{
		Error,
		Success,
		Warning
	}

#endif
}