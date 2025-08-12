using com.ootii.Messages;
using Core.Elements.Windows.Camera.Data;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Debug = Core.Logging.Debug;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("cameras_create")]
	public class CamerasCreate : IncomingMaterialAction
	{
		private CameraMaterialData _material;

		public override bool Parse()
		{
			_material = MaterialParser.ParseOne<CameraMaterialData>(Content);
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
			MessageDispatcher.SendMessage(this, MessageType.CameraAdd, _material, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}