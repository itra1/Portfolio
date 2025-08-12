using Core.Network.Socket.Packets.Incoming.Consts;
using Leguar.TotalJSON;

namespace Core.Network.Socket.Packets.Incoming.Actions.Data
{
    public class StatusContentItemData
    {
        public int Order { get; }
        public ulong AreaId { get; }
        public string Name { get; }
        public ulong MaterialId { get; }
        public int Column { get; }
        public ulong StatusId { get; }
        public ulong ContentId { get; }
        public bool IsActive { get; }
        
        public StatusContentItemData(JSON data)
        {
            Order = data.GetInt(IncomingPacketDataKey.Order);
            AreaId = data.GetULong(IncomingPacketDataKey.AreaId);
            Name = data.GetString(IncomingPacketDataKey.Name);
            MaterialId = data.GetULong(IncomingPacketDataKey.MaterialId);
            Column = data.GetInt(IncomingPacketDataKey.Column);
            StatusId = data.GetULong(IncomingPacketDataKey.StatusId);
            ContentId = data.GetULong(IncomingPacketDataKey.ContentId);
            IsActive = data.GetBool(IncomingPacketDataKey.IsActive);
        }
    }
}