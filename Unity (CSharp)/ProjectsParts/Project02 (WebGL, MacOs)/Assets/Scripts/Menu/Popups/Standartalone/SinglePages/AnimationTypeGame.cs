
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;

public class AnimationTypeGame : MonoBehaviour
{
	[SerializeField] private List<GameObject> typeGame;
	[SerializeField] private int _startIndex = 5;


	private void OnEnable()
	{
		StartCoroutine(NextType());
		typeGame[_startIndex].SetActive(true);
	}


	IEnumerator NextType()
	{
		yield return new WaitForSeconds(5);
		typeGame[_startIndex].SetActive(false);
		int index = 0;
		while (true)
		{
			typeGame.ForEach(x => x.gameObject.SetActive(false));
			typeGame[index].gameObject.SetActive(true);
			index++;
			if (index >= typeGame.Count) index = 0;
			yield return new WaitForSeconds(5);
		}
	}

	public void JackpotButtonTouch()
	{
		it.Main.SinglePageController.Instance.Show(SinglePagesType.Jackpot);
	}
	public void LeaderboardMicroButtonTouch()
	{
#if UNITY_STANDALONE
		PlayerPrefs.SetString(StringConstants.BUTTON_LEADERBOARD_MICRO, "");
		StandaloneController.Instance.FocusMain();
#else

		var go = MobileTablesUIManager.Instance.SelectPage("Leaderboard");
		if (go == null) return;

		var jp = go.GetComponent<it.Main.SinglePages.Leaderboard>();
		jp.VisiblePage(0);
#endif
	}
	public void LeaderboardAverageButtonTouch()
	{
#if UNITY_STANDALONE
		PlayerPrefs.SetString(StringConstants.BUTTON_LEADERBOARD_AVERAGE, "");
		StandaloneController.Instance.FocusMain();
#else
		var go = MobileTablesUIManager.Instance.SelectPage("Leaderboard");
		if (go == null) return;

		var jp = go.GetComponent<it.Main.SinglePages.Leaderboard>();
		jp.VisiblePage(1);
#endif
	}
	public void LeaderboardHighButtonTouch()
	{
#if UNITY_STANDALONE
		PlayerPrefs.SetString(StringConstants.BUTTON_LEADERBOARD_HIGH, "");
		StandaloneController.Instance.FocusMain();
#else
		var go = MobileTablesUIManager.Instance.SelectPage("Leaderboard");
		if (go == null) return;

		var jp = go.GetComponent<it.Main.SinglePages.Leaderboard>();
		jp.VisiblePage(2);
#endif
	}

}
