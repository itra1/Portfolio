using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class GameCenterController : Singleton<GameCenterController> {
	
	string leaderBoardId = "jackrover";
	string leaderBoardIdScore = "jackscore";
	
	void Start() {

		if (!Social.localUser.authenticated) {
			Social.localUser.Authenticate(success => {
				/* (success)
				{
						string userInfo =   "Username: " + Social.localUser.userName +
																"\nUser ID: " + Social.localUser.id +
																"\nIsUnderage: " + Social.localUser.underage;
				}*/
			});
		}
	}

	public void ReportDistance(long distance) {
		if (!Social.localUser.authenticated) return;

		Social.ReportScore(distance, leaderBoardId, success => {
			Debug.Log(success ? "Reported score successfully" : "Failed to report score");
		});
	}


	public void ReportScore(long score) {
		if (!Social.localUser.authenticated) return;

		Social.ReportScore(score, leaderBoardIdScore, success => {
			Debug.Log(success ? "Reported score successfully" : "Failed to report score");
		});
	}

	/*  Показываем лидербоард */
	public static void GameCenterLeaderBoard() {
		Instance.ShowLiderboard();
	}

	public void ShowLiderboard() {
#if UNITY_IOS
		GameCenterPlatform.ShowLeaderboardUI(leaderBoardId, TimeScope.AllTime);
#endif
	}


}
