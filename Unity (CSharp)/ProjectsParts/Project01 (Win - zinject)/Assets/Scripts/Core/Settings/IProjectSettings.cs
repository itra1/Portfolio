using Core.Settings.Server;
using UnityEngine;

namespace Core.Settings
{
	public interface IProjectSettings
	{
		Vector2Int GridSize { get; }
		
		string Server { get; }
		string WebSocket { get; }
		
		ServerData GetServer(ServerType type);
	}
}