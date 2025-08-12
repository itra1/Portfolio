using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using it.Network.Rest;
using it.UI;

public class ChatMess : MonoBehaviour
{
	public UnityEngine.Events.UnityAction<TableChatMessage> OnClickMessage;

	[SerializeField] private TextMeshProUGUI MessageTxt;
	[SerializeField] private TextMeshProUGUI NameTxt;
	[SerializeField] private TextMeshProUGUI TimeTxt;
	[SerializeField] it.UI.Avatar avatar;

	[Space]
	[SerializeField] private GameObject messageGO;
	[SerializeField] private RectTransform _messageParent;
	[SerializeField] private RawImage rawSmile;
	[SerializeField] private float SizeSmileKof = 1.5f;

	private TableChatMessage _message;
	private bool init;

	private UnityEngine.Events.UnityAction<TableChatMessage, Texture2D> OnSmile;

	public TableChatMessage Message { get => _message; set => _message = value; }

	private void Awake()
	{
		if (_message != null && init == false) Init(_message);
	}

	public void InitSet(TableChatMessage messageChat)
	{
		_message = messageChat;
	}
	public void Init(TableChatMessage messageChat, UnityEngine.Events.UnityAction<TableChatMessage, Texture2D> OnSmile = null)
	{
		this.OnSmile = OnSmile;
		Init(messageChat);
	}
	public void Init(TableChatMessage messageChat)
	{
		_message = messageChat;
		User user = messageChat.user;

		NameTxt.text = user.nickname;
		avatar.SetDefaultAvatar();

		//string urlAvatar = user.Id == UserController.User.Id ? UserController.User.AvatarUrl : (!string.IsNullOrEmpty(user.AvatarUrl) ? user.AvatarUrl : "");
		string urlAvatar = user.avatar_url;

		if (!string.IsNullOrEmpty(urlAvatar))
		{
			avatar.SetAvatar(urlAvatar);
		}

		System.DateTime data;
		System.DateTime.TryParse(messageChat.created_at, out data);
		TimeTxt.text = data.ToShortTimeString();

		if (messageChat.message.Contains("smile="))
		{
			int start = messageChat.message.IndexOf("smile=");
			int endsmile = start + "smile=".Length;
			InitSmile(ulong.Parse(messageChat.message.Substring(endsmile, messageChat.message.Length - endsmile)));
		}
		else
		{
			rawSmile.gameObject.SetActive(false);
			MessageTxt.text = messageChat.message;
			RectTransform rt = GetComponent<RectTransform>();
#if UNITY_STANDALONE
			_messageParent.sizeDelta = new Vector2(_messageParent.sizeDelta.x, MessageTxt.preferredHeight + 20);
			rt.sizeDelta = new Vector2(rt.sizeDelta.x, Mathf.Max(_messageParent.sizeDelta.y + 17, 75));
#else
			_messageParent.sizeDelta = new Vector2(_messageParent.sizeDelta.x, MessageTxt.preferredHeight + 60);
			rt.sizeDelta = new Vector2(rt.sizeDelta.x, Mathf.Max(_messageParent.sizeDelta.y +57, 250));
#endif
		}
		init = true;

	}

	public void MessageClick(){
		OnClickMessage?.Invoke(_message);
	}

	public void InitSmile(ulong id)
	{
		messageGO.SetActive(false);
		rawSmile.gameObject.SetActive(false);
		for (int i = 0; i < UserController.ReferenceData.smile_sets.Length; i++)
		{
			SmileSet smileSet = UserController.ReferenceData.smile_sets[i];
			for (int a = 0; a < smileSet.smiles.Count; a++)
			{
				if (smileSet.smiles[a].id == id)
				{
					it.Managers.NetworkManager.Instance.RequestTexture(smileSet.smiles[a].url, (spr, bb) =>
					{
						rawSmile.gameObject.SetActive(true);
						rawSmile.texture = spr;
						OnSmile?.Invoke(_message, spr);
						//if (smile)
						//{
						//imageSmile.SetNativeSize();
						rawSmile.GetComponent<AspectRatioFitter>().aspectRatio = (float)spr.width / (float)spr.height;
						//imageSmile.rectTransform.sizeDelta *= SizeSmileKof;
						//}
					}, null);

					//StartCoroutine(SetImage(smileSet.Smiles[a].Url, imageSmile));
					break;
				}
			}
		}
		//GetComponent<BetterContentSizeFitter>().Source = imageRc;
	}

	//IEnumerator SetImage(string url, Image image, bool smile = false)
	//{

	//	UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
	//	yield return request.SendWebRequest();
	//	if (request.result != UnityWebRequest.Result.Success)
	//		it.Logger.Log(request.error);
	//	else
	//	{
	//		Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
	//		Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
	//		imageSmile.gameObject.SetActive(true);
	//		image.sprite = sprite;
	//		OnSmile?.Invoke(_message, sprite);
	//		if (smile)
	//		{
	//			image.SetNativeSize();
	//			image.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
	//			image.rectTransform.sizeDelta *= SizeSmileKof;
	//		}
	//	}
	//}
}