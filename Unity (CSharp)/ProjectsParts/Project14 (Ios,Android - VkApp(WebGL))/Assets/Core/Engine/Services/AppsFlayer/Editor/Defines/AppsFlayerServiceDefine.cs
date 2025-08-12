using Core.Engine.App.Base.Attributes.Defines;
using Core.Engine.Helpers;
using Core.Engine.Installers;

namespace Core.Engine.Services.AppFlService
{
#if UNITY_EDITOR
	public class AppsFlayerServiceDefine :IToggleDefine
	{
		public string Symbol => "APPSFLAYER_SERVICE";

		public string Description => "ApsFlayer service";

		public void AfterEnable()
		{
			ContextHelpers.AddMonoInstaller<AppsFlayerInstaller>("Assets/Core/Resources/ProjectContext.prefab");
		}

		public void AfterDisable()
		{
			ContextHelpers.RemoveMonoInstaller<AppsFlayerInstaller>("Assets/Core/Resources/ProjectContext.prefab");
		}
	}
#endif
}