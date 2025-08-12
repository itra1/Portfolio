
namespace it.Network.Socket
{
	[SocketAction("replenishment_transaction_updated", "Соьытие обновление баланса игрока")]
	public class ReplenishmentTransactionUpdated : SocketIn
	{
		public ReplenishmentTransaction ReplenishmentTransaction;
		public override void Parse()
		{
			//var socketEventTable = (ReplenishmentTransactionResponse)it.Helpers.ParserHelper.Parse(typeof(ReplenishmentTransactionResponse), JSource.GetJSON("shared_data"));
			var socketEventTable = Newtonsoft.Json.JsonConvert.DeserializeObject<ReplenishmentTransactionResponse>(JSource.GetJSON("shared_data").CreatePrettyString());
			ReplenishmentTransaction = socketEventTable.replenishment_transaction;
		}

		public override void Process()
		{
			com.ootii.Messages.MessageDispatcher.SendMessage(this,
			EventsConstants.ReplenishmentTransactionUpdated, this, 0.05f);

		}

		protected override void Disposing()
		{
			ReplenishmentTransaction = null;
			base.Disposing();
		}
	}
}