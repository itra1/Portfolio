using Cysharp.Threading.Tasks;

namespace Game.Providers.Ui.Presenters.Factorys
{
	public interface IWindowPresenterFactory
	{
		public UniTask<T> GetInstance<T>();
	}
}