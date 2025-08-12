using System;
using Core.Options;
using Cysharp.Threading.Tasks;

namespace Core.Network.Socket.Packets.Outgoing.States.Common
{
	public partial class OutgoingState
	{
		private readonly IApplicationOptions _options;

		private long _timestamp;
		private long _lastTimeSend = 0;
		private bool _waitSend;

		protected override string PacketType => "state";

		public OutgoingState(IApplicationOptions options) => _options = options;

		public void UpdateTimestamp() =>
						system_info.updated_at = UnixTimeMilliseconds;

		private long UnixTimeMilliseconds => ((DateTimeOffset) DateTime.Now).ToUnixTimeMilliseconds();


		public async UniTask AttemptToSendIfAllowed()
		{
			if (!_options.IsStateSendingAllowed)
				return;

			if (_timestamp == system_info.updated_at)
				return;

			if (_waitSend)
				return;
			_waitSend = true;

			// Delay in sending state required
			await UniTask.WaitUntil(() => UnixTimeMilliseconds - _lastTimeSend > 300);

			_lastTimeSend = UnixTimeMilliseconds;
			AttemptToSendIfAllowedSend();

			_timestamp = system_info.updated_at;
			_waitSend = false;
		}
	}
}