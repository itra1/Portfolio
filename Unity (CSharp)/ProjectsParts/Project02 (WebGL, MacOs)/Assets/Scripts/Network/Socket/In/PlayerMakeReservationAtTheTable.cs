
namespace it.Network.Socket
{
	[SocketAction("player_reservation_expired", "Потеря резерва")]
	[SocketAction("player_make_reservation_at_the_table", "Резервирование места")]
	[SocketAction("player_cancel_reservation_at_the_table", "Отмена резерва")]
	public class PlayerMakeReservationAtTheTable : SocketIn, GameUpdate
	{
		public SocketEventTable SocketEventTable;

		public SocketEventTable TableEvent => SocketEventTable;

		public override void Parse()
		{
			IsLockDispose = true;
			//SocketEventTable = (SocketEventTable)it.Helpers.ParserHelper.Parse(typeof(SocketEventTable), JSource.GetJSON("shared_data"));
			SocketEventTable = Newtonsoft.Json.JsonConvert.DeserializeObject<SocketEventTable>(JSource.GetJSON("shared_data").CreatePrettyString());
		}

		public override void Process()
		{
			com.ootii.Messages.MessageDispatcher.SendMessage(this,
			EventsConstants.GameTableReserve(Chanel), this, 0.05f);
		}

		protected override void Disposing()
		{
			SocketEventTable = null;
			base.Disposing();
		}
	}
}