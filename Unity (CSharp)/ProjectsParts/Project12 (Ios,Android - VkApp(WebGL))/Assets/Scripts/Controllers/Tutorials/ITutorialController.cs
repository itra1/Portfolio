using Cysharp.Threading.Tasks;

namespace Game.Scripts.Controllers.Tutorials
{
	public interface ITutorialController
	{
		bool IsReady { get; }

		UniTask Show();
	}
}