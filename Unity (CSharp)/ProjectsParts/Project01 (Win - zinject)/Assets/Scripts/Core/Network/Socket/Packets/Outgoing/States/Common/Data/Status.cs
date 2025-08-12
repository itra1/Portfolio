using System;

namespace Core.Network.Socket.Packets.Outgoing.States.Common.Data
{
    public class Status
    {
        public ulong[] active_colum_material = Array.Empty<ulong>();
        public ulong[] active_materials = Array.Empty<ulong>();
        public ulong active_status_id;
    }
}