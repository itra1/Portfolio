using it.Network.Rest;
using System;

namespace it.Network.Socket
{
	[SocketAction("jackpot_win", "Выйгрыш джекпот")]
	public class JackpotWin : SocketIn
	{
		public ulong UserId;
		public decimal Amount;
		public ulong TableId;

		public override void Parse()
		{
			UserId = JSource.GetJSON("shared_data").GetJNumber("user_id").AsULong();
			Amount = JSource.GetJSON("shared_data").GetJNumber("amount").AsDecimal();
			TableId = JSource.GetJSON("shared_data").GetJNumber("table_id").AsULong();
		}

		public override void Process()
		{
			com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.JackpotWin(Chanel), this, 0.05f);
			Garilla.Jackpot.JackpotController.Instance.SetAwardEvent(TableId, UserId, Amount);
		}
	}
}