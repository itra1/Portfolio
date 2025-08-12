
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;
using PipeServer = Pipes.Server;

namespace OfficeControl.Common
{
	public class PackageProcessor
	{
		private PipeServer _pipeServer;
		private readonly Dictionary<string, Type> _packets = new();

		public PackageProcessor(string serverName)
		{
			_packets = FindActionPackets();
			_pipeServer = new(serverName);
			_ = _pipeServer.Run();
			_ = _pipeServer.WaitConnect();
		}
		~PackageProcessor()
		{
			_ = _pipeServer.Stop();
		}
		public static Dictionary<string, Type> FindActionPackets()
		{
			Dictionary<string, Type> packs = new();

			Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(Package))).ToArray();

			for (int i = 0; i < types.Length; i++)
			{
				object[] ob = types[i].GetCustomAttributes(false);
				for (int x = 0; x < ob.Length; x++)
				{
					if (ob[x] is PackageNameAttribute pName)
					{
						packs.Add(pName.Name, types[i]);
					}
				}
			}
			return packs;
		}

		public async UniTask<Package> Send(Package package)
		{
			string action = "";
			var ob = package.GetType().GetCustomAttributes(false);
			for (var x = 0; x < ob.Length; x++)
			{
				if (ob[x] is PackageNameAttribute pName)
					action = pName.Name;
			}

			(string pachageName, object pachageData) = await _pipeServer.Send(action, package);

			if (!_packets.ContainsKey(pachageName))
			{
				return null;
			}

			var answer = (Package)(JsonConvert.DeserializeObject(pachageData.ToString(), _packets[pachageName]));

			return answer;
		}
	}
}
