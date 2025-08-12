#if UNITY_WEBGL

using System.Runtime.InteropServices;
using Game.Assets.Scripts.Providers.Platforms.Base;

namespace Game.Scripts.Providers.Platforms.Actions
{
	public class WebGlPlatform : IPlatformActions
	{
		public void RestartApplication()
		{
			ReloadBrowser();
		}

		[DllImport("__Internal")]
		private static extern void ReloadBrowser();
	}
}
#endif