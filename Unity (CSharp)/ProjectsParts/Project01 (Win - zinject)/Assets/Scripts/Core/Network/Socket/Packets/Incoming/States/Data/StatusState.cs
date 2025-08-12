using Core.Materials.Attributes;

namespace Core.Network.Socket.Packets.Incoming.States.Data
{
    public class StatusState
    {
        [MaterialDataPropertyParse("active_status_id")]
        public ulong ActiveStatusId { get; set; }
        
        [MaterialDataPropertyParse("active_colum_material")]
        public ulong[] ActiveColumns { get; set; }
        
        [MaterialDataPropertyParse("active_materials")]
        public ulong[] ActiveMaterials { get; set; }
    }
}