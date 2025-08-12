using System.Collections.Generic;

namespace Core.Network.Socket.IgnoredPackets
{
	public class IgnoredIncomingPackets : IIgnoredIncomingPackets
	{
		private readonly ISet<string> _packets;

		public IgnoredIncomingPackets()
		{
			_packets = new HashSet<string>
			{
				"unityrelease_create",
				"unityrelease_delete",
				"onlineUser_update",
				"state_update",
				"capability_update",
				"rates_update",
				"area_update",
				"datasource_update",
				"presentation_create",
				"material_update",
				"episode_create_many",
				"episode_update_many",
				"authdata_create",
				"settings_update",
				"statuscontent_create",
				"statuscontent_delete",
				"statuscontent_update_many",
				"create_preview",
				"netrisrequestentity_create",
				"browser_active_update",
				"notification_create",
				"status_material_set_active",
				"presentationrenderinglog_create",
				"unprocessingfile_create",
				"installation_update",
				"cameraTag_update",
				"userpresetdata_create",
				"userpresetdata_update",
				"socketstatelog_create",
				"socketactionlog_create"
			};
		}

		public bool IsPacketIgnored(string action) => _packets.Contains(action);
	}
}