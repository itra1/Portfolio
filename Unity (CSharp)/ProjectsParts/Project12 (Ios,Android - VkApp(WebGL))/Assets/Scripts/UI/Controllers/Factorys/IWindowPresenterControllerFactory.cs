using Game.Scripts.UI.Controllers.Base;

namespace Game.Scripts.UI.Controllers.Factorys
{
	public interface IWindowPresenterControllerFactory
	{
		void CloseAll();
		T GetInstance<T>();
		IWindowPresenterController GetInstance(string presenterName);
	}
}