using System.Collections.Generic;
using it.Network.Rest;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardInfo : MonoBehaviour
{
	[SerializeField] private Leader[] _leaders;
	[SerializeField] private ScrollRect scrollRect;

	[SerializeField] private GameObject _noData;

	private float hightLeaderPanel;


	private void Start()
	{
		hightLeaderPanel = _leaders[0].GetComponent<RectTransform>().rect.height;
	}

	public void SetData(LeaderBoardResponse leaderBoardResponse)
	{
		if (leaderBoardResponse == null || leaderBoardResponse.data.leaderboard.Length <= 0)
		{
			_noData.SetActive(true);
			for (int i = 0; i < _leaders.Length; i++)
			{
				_leaders[i].gameObject.SetActive(false);
			}
			return;
		}

		int activePlace = 0;
		_noData.gameObject.SetActive(false);
		if (leaderBoardResponse != null && leaderBoardResponse.data.leaderboard.Length > 0)
		{
			//NoData.SetActive(false);
			//var countLeadersInUi = _leaders.Length;
			//var countLeadersFromServer = leaderBoardResponse.data.Length;
			//int countLeadersInUi = countLeadersFromServer;
			for (int i = 0; i < leaderBoardResponse.data.leaderboard.Length; i++)
			{
				_leaders[i].SetData(leaderBoardResponse, i);
			}

			for (int i = 0; i < _leaders.Length; i++)
			{
				if (i < leaderBoardResponse.data.leaderboard.Length)
				{
					activePlace++;
					_leaders[i].gameObject.SetActive(true);
					_leaders[i].SetData(leaderBoardResponse, i);
				}
				else
					_leaders[i].gameObject.SetActive(false);

			}

		}
		else
		{
			_noData.SetActive(false);
			for (int i = 0; i < _leaders.Length; i++)
			{
				_leaders[i].SetDefault();
			}
		}

		if (scrollRect != null)
		{
			scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
				(hightLeaderPanel * activePlace));
		}
	}
}