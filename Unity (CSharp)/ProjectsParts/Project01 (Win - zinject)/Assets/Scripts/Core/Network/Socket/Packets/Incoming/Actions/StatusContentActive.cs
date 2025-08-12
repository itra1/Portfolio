using com.ootii.Messages;
using Core.Materials.Parsing;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Actions.Data;
using Core.Network.Socket.Packets.Incoming.States;
using Zenject;
using Debug = Core.Logging.Debug;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("status_material_set")]
	public class StatusContentActive : IncomingAction
	{
		private readonly IMaterialDataParsingHelper _parsingHelper;
		
		private StatusTabData _data;
		
		public StatusContentActive() =>
			_parsingHelper = ProjectContext.Instance.Container.Resolve<IMaterialDataParsingHelper>();
		
		public override bool Parse()
		{
			_data = _parsingHelper.Parse<StatusTabData>(Content);
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			if (_data == null)
			{
				Debug.LogError($"Active status data is not found when trying to process incoming packet {GetType().Name}");
				return false;
			}

			MessageDispatcher.SendMessage(this, MessageType.StatusTabActive, _data, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}