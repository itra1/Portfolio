using Core.Materials.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions.Data
{
    public class StatusTabData
    {
        [MaterialDataPropertyParse("areaId")] 
        public ulong AreaId { get; set; }
	    
        [MaterialDataPropertyParse("materialId")]
        public ulong MaterialId { get; set; }
	    
        [MaterialDataPropertyParse("column")]
        public int ColumnIndex { get; set; }
	    
        [MaterialDataPropertyParse("statusId")]
        public ulong StatusId { get; set; }
	    
        [MaterialDataPropertyParse("contentId")]
        public ulong ContentId { get; set; }
    }
}