namespace Game.Scripts.UI.Presenters.Factorys
{
	public interface IWindowPresenterFactory
	{
		public T GetInstance<T>();
	}
}