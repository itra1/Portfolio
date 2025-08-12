using System.Collections.Generic;
using System.Linq;
using Core.Settings.Server;
using UnityEngine;

namespace Core.Settings
{
	[CreateAssetMenu(fileName = "ProjectSettings", menuName = "Settings/ProjectSettings", order = 0)]
	public class ProjectSettings : ScriptableObject, IProjectSettings
	{
		[SerializeField] private Vector2Int _gridSize = new (12, 4);
		[SerializeField] private ServerData[] _servers;
		
		private IDictionary<ServerType, ServerData> _serversByType;
		
		public Vector2Int GridSize => _gridSize;
		
		public string Server => GetServer(ServerType).Server;
		public string WebSocket => GetServer(ServerType).WebSocket;
		
		private ServerType ServerType
		{
			get
			{
#if SERVER_STAGE
				return ServerType.Stage;
#elif SERVER_PRODUCT
				return ServerType.Product;
#elif SERVER_LOCAL
				return ServerType.local;
#elif SERVER_DEV_2
				return ServerType.Develop2;
#elif SERVER_SUMEXPO
				return ServerType.SumExpo;
#elif SERVER_SUMEXPOMINI
				return ServerType.SumExpoMini;
#elif SERVER_SYN
				return ServerType.Syn;
#else
				return ServerType.Develop;
#endif
			}
		}
		
		public ServerData GetServer(ServerType type) => 
			_serversByType.TryGetValue(type, out var data) ? data : default;
		
		private void OnEnable() => 
			_serversByType = _servers.ToDictionary(data => data.Type, data => data);
		
		private void OnDisable()
		{
			_serversByType.Clear();
			_serversByType = null;
		}
	}
}