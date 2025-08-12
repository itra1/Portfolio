using Game.Scripts.Providers.Platforms;
using Game.Scripts.Providers.Saves;

namespace Game.Scripts.Helpers
{
	public class PlatformHelper : IPlatformHelper
	{
		private ISaveHandler _saveHelper;
		private IPlatformsProvider _platformProvider;

		public PlatformHelper(ISaveHandler saveHelper, IPlatformsProvider platformProvider)
		{
			_saveHelper = saveHelper;
			_platformProvider = platformProvider;
		}

		public void ClearApplicationProgress()
		{
			_saveHelper.CrearProgress();
			_platformProvider.PlatformActions.RestartApplication();
		}
	}
}
