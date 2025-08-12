using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using it.Network.Rest;
using it.Managers;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class ObserverItemUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI nickname;
	[SerializeField] private RawImage flag;

	public void Init(UserLimited user)
	{
		nickname.text = user.nickname;
		flag.gameObject.SetActive(false);
		NetworkManager.Instance.RequestTexture(user.country.flag, (t, b) =>
		{
			flag.gameObject.SetActive(true);
			flag.texture = t;
		}, null);
		//for (int i = 0; i < UserController.ReferenceData.languages.Count; i++)
		//{
		//	if (UserController.ReferenceData.languages[i].id == user.country.id)
		//	{
		//		//StartCoroutine(SetFlagImage(UserController.ReferenceData.languages[i].flag));

		//		NetworkManager.Instance.RequestTexture(UserController.ReferenceData.languages[i].flag, (t, b) =>
		//		{
		//			flag.texture = t;
		//		}, null);

		//	}
		//}
	}
	//IEnumerator SetFlagImage(string url)
	//{
	//	it.Logger.Log(url);
	//	UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
	//	yield return request.SendWebRequest();
	//	if (request.result != UnityWebRequest.Result.Success)
	//		it.Logger.Log(request.error);
	//	else
	//	{
	//		Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
	//		Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
	//		flag.sprite = sprite;
	//	}
	//}
}
