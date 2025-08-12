using Cysharp.Threading.Tasks;

namespace Game.Providers.Ui.Presenters.Base
{
	public interface IWindowPresenter
	{
		UniTask<bool> Initialize();
	}
}