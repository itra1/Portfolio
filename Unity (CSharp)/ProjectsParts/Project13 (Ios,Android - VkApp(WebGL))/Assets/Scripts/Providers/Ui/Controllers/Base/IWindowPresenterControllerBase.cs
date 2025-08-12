using Cysharp.Threading.Tasks;

namespace Game.Providers.Ui.Controllers.Base
{
	public interface IWindowPresenterControllerBase
	{
		UniTask<bool> Show(IWindowPresenterController source);
		UniTask<bool> Hide();
		UniTask<bool> ParentOpen();
	}
}