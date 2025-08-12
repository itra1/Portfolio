using System.Reflection;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Attributes;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Consts;
using Newtonsoft.Json;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Packets.Converting.Serializers
{
	public class PacketJsonSerializer : IPacketSerializer
	{
		public string Serialize(IPacket packet)
		{
			var result = new object[2];

			var attribute = packet.GetType().GetCustomAttribute<PacketNameAttribute>(false);

			result[0] = attribute != null ? attribute.Name : PacketName.Unknown;
			result[1] = packet;

			return JsonConvert.SerializeObject(result);
		}
	}
}