using Core.Engine.App.Base.Attributes.Defines;
using Core.Engine.Helpers;
using Core.Engine.Installers;

namespace Core.Engine.Services.FBService
{
#if UNITY_EDITOR
	public class FacebookServiceDefine :IToggleDefine
	{
		public string Symbol => "FACEBOOK_SERVICE";
		public string Description => "Facebook service";

		public void AfterEnable()
		{
			ContextHelpers.AddMonoInstaller<FacebookInstaller>("Assets/Core/Resources/ProjectContext.prefab");
		}

		public void AfterDisable()
		{
			ContextHelpers.RemoveMonoInstaller<FacebookInstaller>("Assets/Core/Resources/ProjectContext.prefab");
		}
	}
#endif
}