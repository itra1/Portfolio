using System.Collections;
using UnityEngine;

namespace it.Network.Socket
{
	[SocketAction("balance_updated", "")]
	public class BalanceUpdated : SocketIn
	{

		public decimal Balance;
		public override void Parse()
		{
			Balance = JSource.GetJSON("shared_data").GetJSON("messages").GetJNumber("amount").AsDecimal();
		}

		public override void Process()
		{
			UserController.Instance.UpdateAmount(Balance);
		}

		protected override void Disposing()
		{
			base.Disposing();
		}
	}
}