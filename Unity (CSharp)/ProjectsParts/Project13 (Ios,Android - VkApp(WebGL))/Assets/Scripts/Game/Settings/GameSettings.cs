using Game.Base;
using UnityEngine;

namespace Game.Game.Settings {
	[System.Serializable]
	public partial class GameSettings {
		public GameObject KnifePrefab;
		public GameObject RoundIndicatorPrefab;
		public GameObject BoardPrefab;
		public Texture2D BotAvatar;
		public int MaxItemInHistory = 20;
		public float Speed;
		public int HitPoints;
		public RoundSettings[] HitSettings;
		public PointsModificator PointsModificators;
		public RegisterReward RegisterRewards;
		public CalculablePlayerResources TutorialReward;
		public string BoardFolder;
		public string BonusFolder;
		public string BarrierFolder;
		public string TermsUrl = "https://playoff.cc/terms";
		public string PrivacyUrl = "https://playoff.cc/privacy";

		[System.Serializable]
		public struct PointsModificator {
			public float Increment;
			public float Min;
			public float Max;
		}

		[System.Serializable]
		public struct RegisterReward {
			public CalculablePlayerResources[] Rewards;
		}

		[System.Serializable]
		public struct RoundSettings {
			public float SecondsForHit;
			public float TapForHit;
		}

	}
}
