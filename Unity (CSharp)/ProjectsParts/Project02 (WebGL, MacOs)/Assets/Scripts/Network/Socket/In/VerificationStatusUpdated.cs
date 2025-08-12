using System.Collections;

using UnityEngine;

namespace it.Network.Socket
{
	[SocketAction("verification_status_updated", "")]
	public class VerificationStatusUpdated : SocketIn
	{

		public override void Parse()
		{
		}

		public override void Process()
		{
			UserController.Instance.GetUserProfile();
		}

	}
}