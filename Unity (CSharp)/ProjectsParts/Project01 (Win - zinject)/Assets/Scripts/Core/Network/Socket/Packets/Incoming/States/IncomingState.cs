using Core.App;
using Core.Materials.Parsing;
using Core.Network.Socket.Packets.Incoming.Base;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States.Data;
using Leguar.TotalJSON;
using Zenject;

namespace Core.Network.Socket.Packets.Incoming.States
{
	public class IncomingState : IncomingPacket
	{
		private readonly IMaterialDataParsingHelper _parsingHelper;
		private readonly IApplicationStateAcceptor _applicationState;
		
		private IncomingStateData _state;
		private ulong? _cursorId;
		
		public IncomingState()
		{
			var container = ProjectContext.Instance.Container;
			
			_parsingHelper = container.Resolve<IMaterialDataParsingHelper>();
			_applicationState = container.Resolve<IApplicationStateAcceptor>();
		}
		
		public override bool Parse()
		{
			var dataJson = DataJson;
			
			if (dataJson.ContainsKey(IncomingPacketDataKey.CursorId))
			{
				var jValue = dataJson.Get(IncomingPacketDataKey.CursorId);
				
				if (jValue is not JNull && jValue is JNumber jNumber)
					_cursorId = jNumber.AsULong();
			}
			
			_state = _parsingHelper.ParseClass<IncomingStateData>(dataJson.GetJSON(IncomingPacketDataKey.Data));
			
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			_applicationState.Accept(_state, _cursorId);
			return true;
		}
	}
}