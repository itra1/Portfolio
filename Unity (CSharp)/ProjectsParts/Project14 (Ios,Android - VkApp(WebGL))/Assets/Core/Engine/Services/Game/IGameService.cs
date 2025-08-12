using Core.Engine.App.Base;
using Core.Engine.Services.Common;

namespace Core.Engine.Services
{
	public interface IGameService :IService
	{
		bool IsInitComplete { get; }
		void RunGame();

		void SetGameController(IAppController gameController);

	}
}
