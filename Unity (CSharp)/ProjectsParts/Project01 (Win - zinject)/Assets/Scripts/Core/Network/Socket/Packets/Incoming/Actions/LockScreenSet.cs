using Core.App;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;
using Zenject;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("lock_screen_set")]
	public class LockScreenSet : IncomingAction
	{
		private readonly IApplicationStateSetter _applicationState;

		private bool _isScreenLocked;
		
		public LockScreenSet() => 
			_applicationState = ProjectContext.Instance.Container.Resolve<IApplicationStateSetter>();

		public override bool Parse()
		{
			_isScreenLocked = Content.GetBool(IncomingPacketDataKey.Lock);
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			_applicationState.IsScreenLocked = _isScreenLocked;
			return true;
		}
	}
}