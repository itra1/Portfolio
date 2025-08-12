using Cysharp.Threading.Tasks;
using Game.Scripts.App;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;
using Zenject;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Splash, WindowPresenterNames.Loader)]
	public class LoaderPresenterController : WindowPresenterController<LoaderPresenter>
	{
		private IApplicationLoaderHelper _appLoaderHelper;
		public override bool AddInNavigationStack => false;

		[Inject]
		private void Build(IApplicationLoaderHelper appLoaderHelper)
		{
			_appLoaderHelper = appLoaderHelper;
		}

		public void SetProgress(float value)
		{
			if (Presenter == null)
				return;
			Presenter.LoadingProgress(value);
		}

		public override async UniTask<bool> Open()
		{
			if (!await base.Open())
				return false;

			_appLoaderHelper.OnProgress.AddListener(ChangeProgress);

			return true;
		}

		public override async UniTask<bool> Close()
		{
			if (!await base.Close())
				return false;

			return true;
		}

		private void ChangeProgress(float progress)
		{
			SetProgress(progress);
		}
	}
}
