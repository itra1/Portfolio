using it.Network.Rest;
using System.Collections;
using UnityEngine;

namespace it.Network.Socket
{
	[SocketAction("welcome_bonus_updated", "")]
	public class WelcomeBonusUpdated : SocketIn
	{
		private WelcomeBonusData wb;
		public override void Parse()
		{
			//wb = (WelcomeBonusData)it.Helpers.ParserHelper.Parse(typeof(WelcomeBonusData), JSource.GetJSON("shared_data").GetJSON("messages"));
			wb = Newtonsoft.Json.JsonConvert.DeserializeObject<WelcomeBonusData>(JSource.GetJSON("shared_data").GetJSON("messages").CreatePrettyString());
		}

		public override void Process()
		{
			UserController.Instance.SetWelcomeBonusList(wb);
		}

		protected override void Disposing()
		{
			wb = null;
			base.Disposing();
		}
	}
}