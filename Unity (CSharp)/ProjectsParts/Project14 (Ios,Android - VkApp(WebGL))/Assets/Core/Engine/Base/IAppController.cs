using Core.Engine.Providers;
using Core.Engine.Components.SaveGame;

namespace Core.Engine.App.Base
{
	public interface IAppController
	{
		string AppState { get; }
		ISaveGameProvider SessionProvider { get; }
		void RunGameModule();

		void PlayGame();
	}
}
