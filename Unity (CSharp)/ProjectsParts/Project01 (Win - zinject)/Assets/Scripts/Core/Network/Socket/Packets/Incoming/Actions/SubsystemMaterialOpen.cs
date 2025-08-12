using System.Collections.Generic;
using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("subsystem_material_open")]
	public class SubsystemMaterialOpen : IncomingAction
	{
		private IReadOnlyDictionary<string, (string, string)> _materialsByFolder;
		
		public override bool Parse()
		{
			var json = DataJson.GetJSON(IncomingPacketDataKey.SubsystemMaterialsOpened);
			
			var materialsByFolder = new Dictionary<string, (string, string)>();
			
			foreach (var folder in json.Keys) 
			{
				var subsystemsJson = json.GetJSON(folder);
				
				foreach (var subsystem in subsystemsJson.Keys)
					materialsByFolder.Add(folder, (subsystem, subsystemsJson.GetString(subsystem)));
			}

			_materialsByFolder = materialsByFolder;
			
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.MirMaterialOpen, _materialsByFolder, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}