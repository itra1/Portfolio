namespace Game.Scripts.Managers.Base
{
	public interface IPauseHandler
	{
		bool IsPaused { get; }
		void Pause();
		void UnPause();
	}
}
