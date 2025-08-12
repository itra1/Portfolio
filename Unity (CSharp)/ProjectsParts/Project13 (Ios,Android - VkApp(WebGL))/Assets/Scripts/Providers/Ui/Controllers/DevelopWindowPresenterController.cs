using Cysharp.Threading.Tasks;
using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Controllers.Base;
using Game.Providers.Ui.Presenters;

namespace Game.Providers.Ui.Controllers
{
	[UiController(WindowPresenterType.Popup, WindowPresenterNames.Develop)]
	public class DevelopWindowPresenterController : WindowPresenterController<DevelopWindowPresenter>
	{
		public override async UniTask<bool> Show(IWindowPresenterController source)
		{
			if (!await base.Show(source))
				return false;
			_ = DelayDisable();
			return true;
		}

		private async UniTask DelayDisable()
		{
			await UniTask.Delay(1500);
			_ = Hide();
		}
	}
}
