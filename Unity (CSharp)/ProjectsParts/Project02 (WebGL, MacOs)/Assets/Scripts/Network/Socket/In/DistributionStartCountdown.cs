
using it.Network.Rest;

namespace it.Network.Socket
{
	[SocketAction("distribution_start_countdown", "Запуск отсчета до начала игры")]
	public class DistributionStartCountdown : SocketIn
	{
		public DistributinStartCooldown Cooldown;

		public override void Parse()
		{
			IsLockDispose = true;
			//Cooldown = (DistributinStartCooldown)it.Helpers.ParserHelper.Parse(typeof(DistributinStartCooldown), JSource.GetJSON("shared_data"));
			Cooldown = Newtonsoft.Json.JsonConvert.DeserializeObject<DistributinStartCooldown>(JSource.GetJSON("shared_data").CreatePrettyString());
		}

		public override void Process()
		{
			com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.GameTableCountdown(Chanel), this, 0.05f);
		}

		protected override void Disposing()
		{
			Cooldown = null;
			base.Disposing();
		}
	}
}