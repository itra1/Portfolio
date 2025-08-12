using System.Collections;
using System.Collections.Generic;

using it.Inputs;
using it.Network.Rest;

using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

using Avatar = it.UI.Avatar;

public class Leader : MonoBehaviour
{
	public GameObject leaderPanel;
	public TextMeshProUGUI nickName;
	public CurrencyLabel amount;
	public it.UI.Avatar _avatar;

	[HideInInspector] public TextMeshProUGUI place;
	[HideInInspector] public GameObject placeObj;
	[HideInInspector] public GameObject fifthPlace;

	public bool isFifthPlace;

	public void SetData(LeaderBoardResponse leaderBoardResponse, int index)
	{
		leaderPanel.gameObject.SetActive(true);
		nickName.text = leaderBoardResponse.data.leaderboard[index].nickname;
		amount.SetValue(leaderBoardResponse.data.leaderboard[index].amount);
		_avatar.SetAvatar(leaderBoardResponse.data.leaderboard[index].avatar);

		// SetLastPlace(leaderBoardResponse, index);
	}


	private void SetLastPlace(LeaderBoardResponse leaderBoardResponse, int index)
	{
	}

	public void SetDefault()
	{
		leaderPanel.gameObject.SetActive(false);
	}
}

#if UNITY_EDITOR

[CustomEditor(typeof(Leader))]
public class MAinPlayerFileds : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Leader script = (Leader)target;


		if (script.isFifthPlace)
		{
			script.place =
					EditorGUILayout.ObjectField("Place", script.place, typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
			script.placeObj =
					EditorGUILayout.ObjectField("Place Obj", script.placeObj, typeof(GameObject), true) as
							GameObject;
			script.fifthPlace =
					EditorGUILayout.ObjectField("FifthPlace", script.fifthPlace, typeof(GameObject), true) as
							GameObject;


		}
	}
}

#endif