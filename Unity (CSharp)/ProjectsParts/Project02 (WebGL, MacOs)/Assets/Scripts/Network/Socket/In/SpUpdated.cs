
namespace it.Network.Socket
{
	[SocketAction("sp_updated", "")]
	public class SpUpdated : SocketIn
	{
		RankUser userRank;
		public override void Parse()
		{
			//userRank = (RankUser)it.Helpers.ParserHelper.Parse(typeof(RankUser), JSource.GetJSON("shared_data").GetJSON("messages"));
			userRank = Newtonsoft.Json.JsonConvert.DeserializeObject<RankUser>(JSource.GetJSON("shared_data").GetJSON("messages").CreatePrettyString());
		}

		public override void Process()
		{
			UserController.Instance.SetUserRank(userRank);
		}

		protected override void Disposing()
		{
			userRank = null;
			base.Disposing();
		}
	}
}