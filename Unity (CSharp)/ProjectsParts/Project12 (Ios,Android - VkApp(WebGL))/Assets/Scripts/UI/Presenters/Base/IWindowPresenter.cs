using Cysharp.Threading.Tasks;

namespace Game.Scripts.UI.Presenters.Base
{
	public interface IWindowPresenter
	{
		bool IsVisible { get; }
		UniTask<bool> Initialize();
	}
}