using it.Network.Rest;

using UnityEditor;

namespace it.Network.Socket
{
	[SocketAction("observers_list_updated", "")]
	public class ObserversListUpdated : SocketIn
	{
		public ObserversUsersRespone Obsorves;
		public override void Parse()
		{
			//Obsorves = (ObserversUsersRespone)it.Helpers.ParserHelper.Parse(typeof(ObserversUsersRespone), JSource.GetJSON("shared_data"));
			Obsorves = Newtonsoft.Json.JsonConvert.DeserializeObject<ObserversUsersRespone>(JSource.GetJSON("shared_data").CreatePrettyString());
		}

		public override void Process()
		{
			com.ootii.Messages.MessageDispatcher.SendMessage(this,
			EventsConstants.ObsorveListUpdate(Chanel), this, 0.05f);
		}

		protected override void Disposing()
		{
			base.Disposing();
		}
	}
}