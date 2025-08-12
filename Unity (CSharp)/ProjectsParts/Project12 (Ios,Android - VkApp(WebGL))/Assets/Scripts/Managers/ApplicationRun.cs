using Cysharp.Threading.Tasks;
using Engine.Scripts.Settings;
using Game.Scripts.App;
using Game.Scripts.Controllers.Tutorials;
using Game.Scripts.Providers.Profiles;
using Game.Scripts.UI;
using Game.Scripts.UI.Controllers;
using Zenject;

namespace Game.Scripts.Managers
{
	public class ApplicationRun : IInitializable
	{
		private DiContainer _diContainer;
		private IUiNavigator _uiNavigator;
		private IApplicationLoaderHelper _appLoaderHelper;
		private IProfileProvider _profileProvider;
		private PrefabsLibrary _prefabLibrary;
		private ITutorialController _tutorialController;

		public ApplicationRun(
			DiContainer diContainer,
			IUiNavigator uiNavigator,
			IApplicationLoaderHelper appLoaderHelper,
			IProfileProvider profileProvider,
			PrefabsLibrary prefabLibrary,
			ITutorialController tutorialController
			)
		{
			_diContainer = diContainer;
			_uiNavigator = uiNavigator;
			_appLoaderHelper = appLoaderHelper;
			_profileProvider = profileProvider;
			_prefabLibrary = prefabLibrary;
			_tutorialController = tutorialController;
		}

		public void Initialize()
		{
			_ = InitializeAsync();
		}

		private async UniTask InitializeAsync()
		{
			bool needTutorial = true;

#if DISABLE_TUTORIAL
			needTutorial = false;
#endif

			//CreteInputListener();

			var loaderPresenter = _uiNavigator.GetController<LoaderPresenterController>();
			_uiNavigator.ClearStack();
			_ = await loaderPresenter.Open();

			await _appLoaderHelper.AppLoad();

			await UniTask.Delay(500);

			_uiNavigator.ClearStack();
			if (needTutorial && _tutorialController.IsReady)
			{
				await _tutorialController.Show();
			}
			else
			{
				var homePresenter = _uiNavigator.GetController<HomePresenterController>();
				_ = await homePresenter.Open();
			}

			_ = await loaderPresenter.Close();
		}
		//private void CreteInputListener()
		//{
		//	var listenerInstance = Behaviour.Instantiate(_prefabLibrary.InputListener);
		//	listenerInstance.hideFlags = HideFlags.DontSave;
		//	var component = listenerInstance.GetComponent<IInjection>();
		//	_diContainer.Inject(component);
		//}
	}
}
