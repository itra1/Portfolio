using System.Text.Json.Nodes;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;
using OfficeControl.Pipes.Packages;

namespace OfficeControl.Controllers
{
	/// <summary>
	/// Контроллер обра
	/// </summary>
	internal class PackageManager
	{
		public static PackageManager Instance { get; private set; }
		private Dictionary<string,Type> _packets = new();
		public PackageManager()
		{
			Instance = this;
			_packets = FindActionPackets();
			Console.WriteLine($"PacketsCount {_packets.Count}");
		}

		public static Dictionary<string,Type> FindActionPackets()
		{
			Dictionary<string,Type> packs = new();

			Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(Package))).ToArray();

			for (int i = 0; i < types.Length; i++)
			{
				object[] ob = types[i].GetCustomAttributes(false);
				for (int x = 0; x < ob.Length; x++)
				{
					if (ob[x] is PackageNameAttribute pName)
					{
						packs.Add(pName.Name,types[i]);
					}
				}
			}
			return packs;
		}

		public async Task<string> ProcessMessage(string incoming)
		{

			JsonArray? incomingData = System.Text.Json.JsonSerializer.Deserialize<JsonArray>(incoming);

			string packageName = incomingData[0].GetValue<string>();
			if (!_packets.ContainsKey(packageName))
			{
				return await MakeMessage(new CommonError());
			}

			string jString = incomingData[1].AsObject().ToJsonString();
			Type targetType = _packets[packageName];

			Package? package = (Package)Newtonsoft.Json.JsonConvert.DeserializeObject(jString,targetType);

			if ((typeof(IPackageProcess)).IsAssignableFrom(package.GetType()))
			{
				return await MakeMessage(await PackageProcess(package as IPackageProcess));
			}

			return await MakeMessage(new CommonOk());
		}
		private async Task<Package> PackageProcess(IPackageProcess package)
		{
			return await package.Process();
		}

		private async Task<string> MakeMessage(Package package)
		{

			object[] result = new object[2];
			result[0] = "Unknow";

			object[] ob = package.GetType().GetCustomAttributes(false);
			for (int x = 0; x < ob.Length; x++)
			{
				if (ob[x] is PackageNameAttribute pName)
				{
					result[0] = pName.Name;
				}
			}
			result[1] = package;
			return Newtonsoft.Json.JsonConvert.SerializeObject(result);
		}
		public async Task<string> MakeErrorMessage()
		{
			return await MakeMessage(new CommonError());
		}

	}
}
