using Game.Providers.Ui.Controllers.Base;

namespace Game.Providers.Ui.Controllers.Factorys
{
	public interface IWindowPresenterControllerFactory
	{
		T GetInstance<T>();
		IWindowPresenterController GetInstance(string presenterName);
	}
}