using Core.Materials.Data;
using Core.Network.Socket.Attributes;
using Debug = Core.Logging.Debug;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("folder_create")]
	public class FolderCreate : IncomingMaterialAction
	{
		private FolderMaterialData _material;
		
		public override bool Parse()
		{
			_material = MaterialParser.ParseOne<FolderMaterialData>(Content);
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;

			if (_material == null)
			{
				Debug.LogError($"Material is missing when trying to process incoming packet {GetType().Name}");
				return false;
			}
			
			_material = Materials.Add(_material);
			return true;
		}
	}
}