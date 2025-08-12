using Core.Engine.Components.Vibrations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Components.Vibrations
{
	internal class VibrationEmpty : IVibrationPlatform
	{
		private const string LogKey = "[VIBRATION]";
		public bool IsInitialized { get => true; set { } }

		public void VibrationCancel()
		{
#if UNITY_EDITOR
			AppLog.Log($"{LogKey} VibrationCancel");
#endif
		}

		public bool HasVibrator()
		{
#if UNITY_EDITOR
			AppLog.Log($"{LogKey} HasVibrator");
#endif
			return true;
		}

		public void Init()
		{
#if UNITY_EDITOR
			AppLog.Log($"{LogKey} Init");
#endif
		}

		public void Vibrate()
		{
#if UNITY_EDITOR
			AppLog.Log($"{LogKey} Vibrate");
#endif
		}

		public void VibrateNope()
		{
#if UNITY_EDITOR
			AppLog.Log($"{LogKey} VibrateNope");
#endif
		}

		public void VibratePeek()
		{
#if UNITY_EDITOR
			AppLog.Log($"{LogKey} VibratePeek");
#endif
		}

		public void VibratePop()
		{
#if UNITY_EDITOR
			AppLog.Log($"{LogKey} VibratePop");
#endif
		}
	}
}
