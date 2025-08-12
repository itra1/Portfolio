using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateTableConfig : MonoBehaviour
{

	[SerializeField] GameObject _info;
	[SerializeField] GameObject _tools;
	[SerializeField] GameObject _parent;
	[SerializeField] ScrollRect scrollRect;

	[SerializeField] TypeGame _typeGames;

	//private CreateTablePrefabs[] currentObj;

	private void OnEnable()
	{
		ShowPanel(_info);
	}

	public void ShowPanel(GameObject gameObject)
	{
		//var objects = _parent.GetComponentsInChildren<CreateTablePrefabs>();

		//for (int i = 0; i < objects.Length; i++)
		//{
		//	Destroy(objects[i].gameObject);
		//}
		SpawnObjects(gameObject);
	}

	private void SpawnObjects(GameObject gameObject)
	{
		var a = _typeGames.GetComponent<RectTransform>().rect.height / 2;
		var b = gameObject.GetComponent<RectTransform>().rect.height / 2;
		var c = scrollRect.GetComponent<ScrollRect>().content.sizeDelta;
		if (gameObject == _tools)
		{
			scrollRect.content.GetComponent<VerticalLayoutGroup>().spacing = 0;
			scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
						((a + b) - 900));
		}
		else
		{
			scrollRect.content.GetComponent<VerticalLayoutGroup>().spacing = -100;
			scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x,
					 ((a + b) - 800));
		}

		var obj = Instantiate(_typeGames, _parent.transform);
		var obj1 = Instantiate(gameObject, _parent.transform);

		for (int i = 0; i < obj.games.Count; i++)
		{
			obj.games[i].onClick.RemoveAllListeners();
			obj.games[i].onClick.AddListener(() => ShowPanel(_tools));
		}
	}

}











