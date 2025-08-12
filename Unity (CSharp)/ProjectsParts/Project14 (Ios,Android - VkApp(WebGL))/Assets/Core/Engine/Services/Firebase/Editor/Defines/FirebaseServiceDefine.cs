using Core.Engine.App.Base.Attributes.Defines;
using Core.Engine.Helpers;
using Core.Engine.Installers;

namespace Core.Engine.Services.FireBService
{
#if UNITY_EDITOR
	public class FirebaseServiceDefine :IToggleDefine
	{
		public string Symbol => "FIREBASE_SERVICE";

		public string Description => "Firebase service";

		public void AfterEnable()
		{
			ContextHelpers.AddMonoInstaller<FirebaseInstaller>("Assets/Core/Resources/ProjectContext.prefab");
		}

		public void AfterDisable()
		{
			ContextHelpers.RemoveMonoInstaller<FirebaseInstaller>("Assets/Core/Resources/ProjectContext.prefab");
		}
	}
#endif
}