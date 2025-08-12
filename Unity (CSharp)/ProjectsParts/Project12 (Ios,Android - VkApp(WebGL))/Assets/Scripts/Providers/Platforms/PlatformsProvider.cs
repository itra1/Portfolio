using Game.Assets.Scripts.Providers.Platforms.Base;
using Game.Scripts.Providers.Platforms.Actions;

namespace Game.Scripts.Providers.Platforms
{
	public class PlatformsProvider : IPlatformsProvider
	{
		private IPlatformActions _platformActions;
		public IPlatformActions PlatformActions => _platformActions;

		public PlatformsProvider()
		{
#if UNITY_EDITOR
			_platformActions = new EditorPlatform();
#elif UNITY_WEBGL
			_platformActions = new WebGlPlatform();
#else
			throw new System.NotImplementedException("No support current platform");
#endif
		}

	}
}
