using com.ootii.Messages;
using Core.Materials.Data;
using Core.Materials.Storage;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;
using Zenject;
using Debug = Core.Logging.Debug;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("statuscontent_update")]
	public class StatusContentUpdate : IncomingAction
	{
		private readonly IMaterialDataStorage _materials;
		
		public int Order { get; private set; } //Starts at value 1, not 0
		public ulong AreaId { get; private set; }
		public ulong MaterialId { get; private set; }
		public int Column { get; private set; } //Starts at value 1, not 0
		public ulong StatusId { get; private set; }
		public ulong ContentId { get; private set; }
		
		public StatusContentUpdate() => _materials = ProjectContext.Instance.Container.Resolve<IMaterialDataStorage>();
		
		public override bool Parse()
		{
			Order = Content.GetInt(IncomingPacketDataKey.Order);
			AreaId = Content.GetULong(IncomingPacketDataKey.AreaId);
			MaterialId = Content.GetULong(IncomingPacketDataKey.MaterialId);
			Column = Content.GetInt(IncomingPacketDataKey.Column);
			StatusId = Content.GetULong(IncomingPacketDataKey.StatusId);
			ContentId = Content.GetULong(IncomingPacketDataKey.ContentId);
            
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			var contentAreaMaterial = _materials.Get<ContentAreaMaterialData>(AreaId);
			
			if (contentAreaMaterial == null)
			{
				Debug.LogError($"An instance of ContentAreaMaterialData is not found by area id {AreaId} when trying to process incoming packet {GetType().Name}");
				return false;
			}
			
			contentAreaMaterial.Order = Order;
			
			MessageDispatcher.SendMessage(this, MessageType.StatusOrderUpdate, this, EnumMessageDelay.NEXT_UPDATE);
			return true;
		}
	}
}