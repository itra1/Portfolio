
using it.Network.Rest;
using TMPro;
using UnityEngine;

public class MainLeader : MonoBehaviour
{
	[SerializeField] public GameObject leaderNickName;
	[SerializeField] public TextMeshProUGUI beTheFirst;
	[SerializeField] public TextMeshProUGUI nick;
	[SerializeField] public TextMeshProUGUI money;
	[SerializeField] public it.UI.Avatar avatar;
	public void SetMainPlayer(LeaderBoardResponseData leaderBoardResponse)
	{
		if (leaderBoardResponse == null)
		{
			SetDefault();
			return;
		}

		nick.text = leaderBoardResponse.nickname;
		money.gameObject.SetActive(true);
		//money.SetValue(leaderBoardResponse.data[0].amount);
		avatar.SetAvatar(leaderBoardResponse.avatar);
		leaderNickName.SetActive(true);
		beTheFirst.gameObject.SetActive(false);

		double val = leaderBoardResponse.prize;
		double cell = System.Math.Floor(val);
		double prop = val - cell; ;

		money.text = $"{StringConstants.CURRENCY_SYMBOL}{it.Helpers.Currency.String(cell, false)}.<size=50%>{System.Math.Floor(prop * 100)}";

	}

	public void SetDefault()
	{
		// leaderPanel.gameObject.SetActive(false);

		leaderNickName.SetActive(false);
		beTheFirst.gameObject.SetActive(true);
		money.gameObject.SetActive(false);
		avatar.SetDefaultAvatar();

	}
}