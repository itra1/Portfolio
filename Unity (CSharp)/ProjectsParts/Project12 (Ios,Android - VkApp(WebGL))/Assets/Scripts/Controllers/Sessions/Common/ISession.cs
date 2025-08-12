using Engine.Engine.Scripts.Managers.Interfaces;
using Game.Scripts.Controllers.Sessions.Debugs;

namespace Game.Scripts.Controllers.Sessions.Common
{
	public interface ISession : IDebugSession, IGamePlaying
	{
		string SceneVisibleMode { get; }

		void SceneVisibleMoveSet(string value);
		void SceneVisibleToggle();
	}
}