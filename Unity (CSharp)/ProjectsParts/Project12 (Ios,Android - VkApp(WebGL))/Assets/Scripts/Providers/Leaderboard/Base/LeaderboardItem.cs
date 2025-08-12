namespace Game.Scripts.Providers.Leaderboard.Base
{
	[System.Serializable]
	public class LeaderboardItem
	{
		public int Index;
		public string Nickname;
		public float Value;
		public string AvatarUuid;
		public bool IsMe { get; set; }
	}
}
