using System.Collections;
using UnityEngine;

namespace it.Network.Socket
{
	[SocketAction("login", "Событие авторизации на другом устройстве")]
	public class Login : SocketIn
	{
		public override void Parse(){	}

		public override void Process()
		{
			UserController.Instance.AnotherPlayerAuthorization();
		}

		protected override void Disposing()
		{
			base.Disposing();
		}
	}
}