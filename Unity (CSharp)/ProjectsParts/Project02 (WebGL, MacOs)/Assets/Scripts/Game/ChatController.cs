using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using it.Network.Rest;
using DG.Tweening;
using it.Network.Socket;
using System.Linq;
using it.Mobile;
using com.ootii.Geometry;
using Garilla;
using Garilla.Games;

public class ChatController : MonoBehaviour
{
	[SerializeField] private it.UI.GamePanel _gamePanel;
	[SerializeField] private RectTransform _chatRT;
	[SerializeField] private TMP_InputField InputMessageField;
	[SerializeField] private ScrollRect _chatScrollRect;
	[SerializeField] private it.UI.Elements.GraphicButtonUI buttonSend;
	[SerializeField] private GameObject _infoIcone;
	[SerializeField] private it.UI.PlayerChatSmile _playerSmile;
	[SerializeField] private it.UI.SmilePresets _smilePresetFixed;
	[SerializeField] private RectTransform _dropSmilePrefab;

	[Space]
	[SerializeField] private ChatMess MessagePref;
	[SerializeField] private ChatMess MessageRightPref;
	private List<ChatMess> _messes = new List<ChatMess>();
	[SerializeField] private it.UI.SmilePresets _smilePreset;

	[Space, Header("Smile")]
	[SerializeField] private RectTransform _smileRT;
	[SerializeField] private ScrollRect _smileScrollRect;
	[SerializeField] private Button SmileButtonPref;
	private List<Button> SmileButtons = new List<Button>();
	private List<Smile> Smiles = new List<Smile>();
	[SerializeField] private float MaxScrollSmileLoad = 0.2f;

	private bool _autoscroll;
	private Table _table;
	private ReferenceData _referenceData;
	private bool _isInit;
	private bool _isInitChat;
	private bool _chatVisible;
	private bool _smileVisible;
	private string _chanel;
	private CanvasGroup _chatCanvas;
	private CanvasGroup _smilesCanvas;

	private const string SmilePrefsPrefix = "Smile";
	[SerializeField] private ScrollRect _commonSmilesScrollrect;
	bool onlySmile;

	private void Awake()
	{
		_chatCanvas = _chatRT.gameObject.GetOrAddComponent<CanvasGroup>();
		_chatRT.gameObject.GetOrAddComponent<GraphicRaycaster>();
		_chatCanvas.alpha = 0;
		_smilesCanvas = _smileRT.gameObject.GetOrAddComponent<CanvasGroup>();
		_smileRT.gameObject.GetOrAddComponent<GraphicRaycaster>();
		_smilesCanvas.alpha = 0;
	}

	public void Init(Table table, bool chat, bool smile)
	{
		_table = table;
		_autoscroll = true;
		_chanel = SocketClient.GetChanelTable(_table.id);
		if (_infoIcone != null)
			_infoIcone.gameObject.SetActive(false);


		if (_isInit == false)
		{
			_isInit = true;
			//_chatScrollRect.content.GetComponent<BetterAxisAlignedLayoutGroup>().callbackVerticalChange += ScrollDown;
			//WebsocketClient.Instance.MessagePostedCallback += AddMessage;
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.ChatMessage(_chanel), ChatHandler);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ChatMessage(_chanel), ChatHandler);


		}
		if (_isInitChat == false && chat == true)
		{
			_isInitChat = true;
			GameHelper.ChatTableOpen(table, ChatCreate);
		}
		ReferenceLoad(UserController.ReferenceData);

		MessEdit(InputMessageField.text);
		_chatVisible = false;
		_smileVisible = false;
		//_chatRT.anchoredPosition = new Vector2(chat ? 0 : -420, _chatRT.anchoredPosition.y);
		//_smileRT.anchoredPosition = new Vector2(chat ? 0 : -420, _smileRT.anchoredPosition.y);
		if (smile) LoadUsedSmiles();
		onlySmile = chat == false && smile == true;
		ScrollDown();
	}

	private void Update()
	{
		if (_chatVisible && Input.GetKeyUp(KeyCode.Return) && Input.GetKey(KeyCode.LeftControl) == false)
		{
			PostChatFromField();
			UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(InputMessageField.gameObject);
			InputMessageField.ActivateInputField();
		}
	}

	private void OnDestroy()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.ChatMessage(_chanel), ChatHandler);
	}

	private void ChatHandler(com.ootii.Messages.IMessage handel)
	{
		if (!AppConfig.DisableAudio)
			DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_CHAT, 1, null, 0, StringConstants.SOUND_CHAT_RECIEVE);

		AddMessage(((ChatMessagePosted)handel.Data).Message, true, true);
	}

	public void ChatOpen()
	{
		if (_chatVisible) return;
		if (_infoIcone != null)
			_infoIcone.gameObject.SetActive(false);

		_chatCanvas.alpha = 1;
		_chatRT.DOAnchorPos(new Vector2(0, _chatRT.anchoredPosition.y), 0.3f);
		ScrollDown();

#if !UNITY_STANDALONE

		var tmp = _chatRT.GetComponentInChildren<TopMobilePanel>(true);
		if (tmp != null)
		{
			tmp.OnBackAction.AddListener(ChatClose);
			tmp.SwipeListenerAdd();
		}
#endif

		UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(InputMessageField.gameObject);
		InputMessageField.ActivateInputField();
		_chatVisible = true;
	}

	public void ChatClose()
	{
		if (!_chatVisible) return;
		//if (SmileWindow.activeInHierarchy) SmileWindowClose();
		//SmileButtons.ForEach(x => { Destroy(x.gameObject); });
		//SmileButtons.Clear();
		//Smiles.Clear();
		_chatRT.DOAnchorPos(new Vector2(-_chatRT.rect.width - 200, _chatRT.anchoredPosition.y), 0.3f).OnComplete(() => { _chatCanvas.alpha = 0; });
		_chatVisible = false;
		_autoscroll = false;

#if !UNITY_STANDALONE
		var tmp = _chatRT.GetComponentInChildren<TopMobilePanel>(true);
		if (tmp != null)
		{
			tmp.SwipeListenerRemove();
		}
#endif
		//gameObject.SetActive(false);
	}

	private void ChatCreate(List<TableChatMessage> messages)
	{
		_messes.ForEach(x => { Destroy(x.gameObject); });
		_messes.Clear();
		messages = messages.OrderBy(x => x.CreateDate).ToList();
		for (int i = 0; i < messages.Count; i++)
		{
			AddMessage(messages[i], false);
		}
		DOVirtual.DelayedCall(0.1f, () =>
		{
			ScrollDown();
		});
		if (_infoIcone != null)
			_infoIcone.gameObject.SetActive(false);
	}

	private void ScrollDown()
	{
		if (_autoscroll)
			_chatScrollRect.verticalScrollbar.value = 0;
	}

	public void SetAutoScroll(float vl)
	{
		_autoscroll = vl == 0;
	}

	private void AddMessage(TableChatMessage message, bool sendTableSmile = true, bool live = false)
	{
		if (_messes.Exists(x => x.Message.id == message.id)) return;

		ChatMess mess;
		if (message.user.id == GameHelper.UserInfo.id)
		{
			mess = Instantiate(MessageRightPref, _chatScrollRect.content);
		}
		else
		{
			mess = Instantiate(MessagePref, _chatScrollRect.content);
		}

		if (!_chatVisible && message.user.id != GameHelper.UserInfo.id)
			if (_infoIcone != null)
				_infoIcone.gameObject.SetActive(true);

		//if (first)
		//{
		mess.transform.SetAsLastSibling();
		//}
		mess.gameObject.SetActive(true);
		_messes.Add(mess);

		if (!message.message.Contains("smile=") && _gamePanel.GameSession.SettingsShowMessage)
		{
			foreach (var key in _gamePanel.CurrentGameUIManager.Players.Keys)
			{
				if (_gamePanel.CurrentGameUIManager.Players[key].UserId == message.user_id)
				{
					_gamePanel.CurrentGameUIManager.Players[key].SetDialog(message.message);
				}
			}
		}

		if (onlySmile)
			mess.InitSet(message);
		else
			mess.Init(message, sendTableSmile ? SmileMessage : null);
		mess.OnClickMessage = (m) =>
		{
			if (m.message.Contains("smile=")) return;
			InputMessageField.text = $"-\"{m.message}\"\n" + InputMessageField.text;
		};
		Canvas.ForceUpdateCanvases();

		if (onlySmile) ChatClose();

		float h = 0;
		for (int i = 0; i < _chatScrollRect.content.childCount; i++)
		{
			h += _chatScrollRect.content.GetChild(i).GetComponent<RectTransform>().rect.height;
		}
		h += _chatScrollRect.content.GetComponent<VerticalLayoutGroup>().spacing * (_chatScrollRect.content.childCount);
		_chatScrollRect.content.sizeDelta = new Vector2(_chatScrollRect.content.sizeDelta.x, h);
		Canvas.ForceUpdateCanvases();

		DOVirtual.DelayedCall(0.1f, () =>
		{
			_chatScrollRect.verticalScrollbar.value = 0;
		});

		//Canvas.ForceUpdateCanvases();
	}
	private double _lastUse;
	public void ShowMySmilePresets()
	{

		//double foldTime = Time.timeAsDouble - _gamePanel.GameSession.TimeMyFold;

		//if (foldTime < 2)
		//{
		//	DOVirtual.DelayedCall((float)foldTime, () => { ShowMySmilePresets(); });
		//	return;
		//}

		if (!_gamePanel.GameSession.SettingsShowEmojies) return;

		if (Time.timeAsDouble - _lastUse < 5) return;
		_lastUse = Time.timeAsDouble;

		var players = _gamePanel.CurrentGameUIManager.Players;


		foreach (var elem in players.Values)
		{
			if (elem.UserId == UserController.User.id)
			{
				if (_smilePresetFixed != null)
				{
					_smilePresetFixed.SetData(this);
					_smilePresetFixed.gameObject.SetActive(true);
					continue;
				}

				GameObject inst = Instantiate(_smilePreset.gameObject, elem.transform.parent);
				inst.transform.SetAsLastSibling();
				var compon = inst.GetComponent<it.UI.SmilePresets>();
				inst.transform.localScale = Vector3.one;
				inst.transform.localPosition = new Vector2(0, 0);
#if !UNITY_STANDALONE
				inst.transform.localPosition = new Vector2(0, 20);
				inst.transform.localScale = Vector3.one * 1.5f;
#endif
				inst.transform.localRotation = Quaternion.identity;
				compon.SetData(this);
				inst.gameObject.SetActive(true);
				//Destroy(inst, 2);
			}
		}
	}

	public void PostChat(string mess)
	{
		if (mess.Length == 0) return;
		_autoscroll = true;
		if (!AppConfig.DisableAudio)
			DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget(StringConstants.SOUND_CHAT, 1, null, 0, StringConstants.SOUND_CHAT_SEND);

		GameHelper.ChatTablePost(_table, new TableChatPostMessage { message = mess }, AddMessage);
	}

	public void DropSmileSend(ulong userId, ulong smileId, UnityEngine.Events.UnityAction<bool> OnComplete)
	{

		TableApi.DropSmie(_table.id, userId, smileId, (res) =>
		{
			OnComplete?.Invoke(res.IsSuccess);
		});

	}

	public void DropSmileActive(ulong fromUserId, ulong toUserId, ulong smileId)
	{

		Debug.Log("Activate drop smile");

		Smile sm = null;

		for (int i = 0; i < UserController.ReferenceData.smiles_to_send.Length; i++)
		{
			if (UserController.ReferenceData.smiles_to_send[i].id == smileId)
				sm = UserController.ReferenceData.smiles_to_send[i];
		}

		if (sm == null) return;

		it.Managers.NetworkManager.Instance.RequestTexture(sm.url, (t, b) =>
		{
			PlayerGameIcone fromPlayer = null;
			PlayerGameIcone toPlayer = null;

			foreach (var elem in _gamePanel.CurrentGameUIManager.Players)
			{
				if (elem.Value.UserId == fromUserId)
					fromPlayer = elem.Value;
				if (elem.Value.UserId == toUserId)
					toPlayer = elem.Value;
			}

			if (fromPlayer == null || toPlayer == null) return;

			GameObject inst = Instantiate(_dropSmilePrefab.gameObject, fromPlayer.transform);
			inst.gameObject.SetActive(true);
			RawImage ri = inst.gameObject.GetComponentInChildren<RawImage>();
			ri.texture = t;
			var arf = ri.GetComponent<AspectRatioFitter>();
			arf.aspectRatio = (float)t.width / (float)t.height;
			RectTransform rInst = inst.GetComponent<RectTransform>();
			rInst.localScale = Vector3.one;
			rInst.anchoredPosition = Vector2.zero;
			rInst.localScale = Vector3.zero;
			rInst.SetParent(toPlayer.transform);
			rInst.DOScale(Vector3.one, 0.5f);
			rInst.DOAnchorPos(Vector3.zero, 0.5f).OnComplete(() =>
			{

				rInst.DOScale(Vector3.zero, 0.3f).SetDelay(1f).OnComplete(() =>
				{
					Destroy(inst, 1f);
				});

			});

		}, null);
	}

	/// <summary>
	/// Отображение смайлика поверх иконки пользователя
	/// </summary>
	/// <param name="message">Сообщение чата</param>
	/// <param name="sprite">Спрайт смайлика</param>
	private void SmileMessage(TableChatMessage message, Texture2D sprite)
	{
		if (!_gamePanel.GameSession.SettingsShowEmojies) return;
		var players = _gamePanel.CurrentGameUIManager.Players;

		foreach (var elem in players.Values)
		{
			if (elem.UserId == message.user_id)
			{
				GameObject inst = Instantiate(_playerSmile.gameObject, elem.transform.parent);
				inst.transform.SetAsLastSibling();
				var compon = inst.GetComponent<it.UI.PlayerChatSmile>();
				compon.SetTexture(sprite);
				inst.transform.localScale = Vector3.one;
#if !UNITY_STANDALONE
				inst.transform.localScale = Vector3.one * 1.5f;
#endif
				inst.transform.localPosition = new Vector2(0, 20);
				inst.transform.SetParent(_gamePanel.CurrentGameUIManager.transform);

				inst.gameObject.SetActive(true);
				//Destroy(inst, 2);
			}
		}

	}

	public void PostChatFromField()
	{
		PostChat(InputMessageField.text);

		//string url = $"/tables/{table.Id}/chat";

		//it.Managers.NetworkManager.Request()

		InputMessageField.text = "";
	}

	public void SmileWindowOpen()
	{
		if (_smileVisible)
			return;

		_smileVisible = true;
		_smilesCanvas.alpha = 1;
		_smileRT.DOAnchorPos(new Vector2(0, _smileRT.anchoredPosition.y), 0.3f);

#if !UNITY_STANDALONE

		var tmp = _smileRT.GetComponentInChildren<TopMobilePanel>(true);
		if (tmp != null)
		{
			tmp.OnBackAction.AddListener(ClickCloseSmileWindow);
			tmp.SwipeListenerAdd();
		}
#endif

		//SmileWindow.SetActive(true);
		//LoadUsedSmiles();
	}

	public void ClickCloseSmileWindow()
	{
		if (onlySmile) ChatClose();
		else SmileWindowClose();
	}

	public void SmileWindowClose()
	{
		_smileRT.DOAnchorPos(new Vector2(-_smileRT.rect.width - 200, _smileRT.anchoredPosition.y), 0.3f).OnComplete(() => { _smilesCanvas.alpha = 0; });
		_smileVisible = false;

#if !UNITY_STANDALONE
		var tmp = _smileRT.GetComponentInChildren<TopMobilePanel>(true);
		if (tmp != null)
		{
			tmp.SwipeListenerRemove();
		}
#endif

		//SmileWindow.SetActive(false);
	}

	public void ReferenceLoad(ReferenceData referenceData)
	{
		this._referenceData = referenceData;

		for (int i = 0; i < referenceData.smile_sets.Length; i++)
		{
			for (int a = 0; a < referenceData.smile_sets[i].smiles.Count; a++)
			{
				Smiles.Add(referenceData.smile_sets[i].smiles[a]);
			}
		}

		for (int i = 0; i < Smiles.Count; i++)
		{
			int index = i;

			it.Managers.NetworkManager.Instance.RequestTexture(Smiles[index].url, (spr, bb) =>
			{
				Button buttonSmile = Instantiate(SmileButtonPref, _smileScrollRect.content);
				var img = buttonSmile.transform.GetChild(1).GetComponentInChildren<RawImage>();
				img.texture = spr;
				img.GetComponent<AspectRatioFitter>().aspectRatio = (float)spr.width / (float)spr.height;
				buttonSmile.onClick.AddListener(() => UseSmile((int)Smiles[index].id));
				SmileButtons.Add(buttonSmile);

			}, null);

		}

		//StartCoroutine(DownloadImages());
		var glg = _smileScrollRect.content.GetComponent<GridLayoutGroup>();
		int row = (int)Mathf.Ceil(Smiles.Count / 6);
		float h = row * glg.cellSize.y + ((row - 1) * glg.spacing.y);
		h += glg.padding.bottom;
		h += glg.padding.top;
		//h = Mathf.Min(h, 1000);
		_smileScrollRect.content.sizeDelta = new Vector2(_smileScrollRect.content.sizeDelta.x, h);
	}

	//IEnumerator DownloadImages()
	//{
	//	for (int i = 0; i < Smiles.Count; i++)
	//	{
	//		//while (_smileScrollRect.verticalScrollbar.value > MaxScrollSmileLoad)
	//		//{
	//		//	yield return new WaitForEndOfFrame();
	//		//}
	//		UnityWebRequest request = UnityWebRequestTexture.GetTexture(Smiles[i].Url);
	//		yield return request.SendWebRequest();
	//		if (request.result == UnityWebRequest.Result.ConnectionError || request.isHttpError)
	//		{

	//			it.Logger.Log(request.error);
	//		}
	//		else
	//		{
	//			AddSmile(((DownloadHandlerTexture)request.downloadHandler).texture, (int)Smiles[i].Id);
	//		}
	//	}
	//}

	void AddSmile(Texture2D texture, int id)
	{
		Button buttonSmile = Instantiate(SmileButtonPref, _smileScrollRect.content);
		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
		buttonSmile.transform.GetChild(1).GetComponent<Image>().overrideSprite = sprite;
		buttonSmile.onClick.AddListener(() => UseSmile(id));
		SmileButtons.Add(buttonSmile);
	}

	public void UseSmile(int id)
	{
		SaveSmile(id);
		PostChat("smile=" + id);
		SmileWindowClose();
		it.Logger.Log($"UseSmile({id})");
	}

	public void MessEdit(string txt)
	{
		//buttonSend.interactable = txt.Length > 0;
	}


	private void SaveSmile(int SmileId)
	{
		for (int c = 0; c < 5; c++)
		{
			if (PlayerPrefs.HasKey(SmilePrefsPrefix + c.ToString()))
			{
				if (PlayerPrefs.GetInt(SmilePrefsPrefix + c.ToString()) == SmileId)
				{
					it.Logger.Log("[Chat:SmileWindow] common smile duplicate, abort");
					return;
				}
			}
		}

		for (int a = 0; a < 5; a++)
		{
			if (!PlayerPrefs.HasKey(SmilePrefsPrefix + a.ToString()))
			{
				PlayerPrefs.SetInt(SmilePrefsPrefix + a.ToString(), SmileId);
				it.Logger.Log("[Chat:SmileWindow] smile saved, no list movement; SmileId = " + SmileId);
				return;
			}
		}


		for (int b = 4; b >= 1; b--)
		{
			PlayerPrefs.SetInt(SmilePrefsPrefix + (b).ToString(), PlayerPrefs.GetInt(SmilePrefsPrefix + (b - 1).ToString()));
		}


		it.Logger.Log("[Chat:SmileWindow] smile saved, list movement; SmileId = " + SmileId);
		PlayerPrefs.SetInt(SmilePrefsPrefix + 0.ToString(), SmileId);
	}

	private void LoadUsedSmiles()
	{
		for (int a = 0; a < _commonSmilesScrollrect.content.childCount; a++)
		{
			Destroy(_commonSmilesScrollrect.content.GetChild(a).gameObject);
		}

		for (int c = 0; c < 5; c++)
		{
			if (PlayerPrefs.HasKey(SmilePrefsPrefix + c.ToString()))
			{
				int theId = PlayerPrefs.GetInt(SmilePrefsPrefix + c.ToString());
				var sm = _referenceData.smile_sets[0].smiles.Find(x => x.id == (ulong)theId);
				//int index = i;
				it.Managers.NetworkManager.Instance.RequestTexture(sm.url, (spr, bb) =>
				{

					Button buttonSmile = Instantiate(SmileButtonPref, _commonSmilesScrollrect.content);
					var img = buttonSmile.transform.GetChild(1).GetComponentInChildren<RawImage>();
					img.texture = spr;
					img.GetComponent<AspectRatioFitter>().aspectRatio = (float)spr.width / (float)spr.height;
					buttonSmile.onClick.AddListener(() => UseSmile(theId));
					SmileButtons.Add(buttonSmile);

				}, null);




				//StartCoroutine(AddUsedSmile(theId));
			}
		}
	}

	//IEnumerator AddUsedSmile(int smileId)
	//{
	//	if (_referenceData.SmileSets == null) yield break;

	//	for (int i = 0; i < _referenceData.SmileSets.Length; i++)
	//	{
	//		for (int a = 0; a < _referenceData.SmileSets[i].Smiles.Length; a++)
	//		{
	//			if (_referenceData.SmileSets[i].Smiles[a].Id == smileId)
	//			{
	//				UnityWebRequest request = UnityWebRequestTexture.GetTexture(_referenceData.SmileSets[i].Smiles[a].Url);
	//				yield return request.SendWebRequest();
	//				if (request.result == UnityWebRequest.Result.ConnectionError || request.isHttpError)
	//				{
	//					it.Logger.Log(request.error);
	//				}
	//				else
	//				{
	//					var textureHandle = ((DownloadHandlerTexture)request.downloadHandler).texture;

	//					Button buttonSmile = Instantiate(SmileButtonPref, _commonSmilesScrollrect.content);
	//					Sprite sprite = Sprite.Create(textureHandle, new Rect(0, 0, textureHandle.width, textureHandle.height), new Vector2(textureHandle.width / 2, textureHandle.height / 2));
	//					buttonSmile.transform.GetChild(1).GetComponent<Image>().overrideSprite = sprite;
	//					buttonSmile.onClick.AddListener(() => UseSmile(smileId));
	//				}
	//			}
	//		}
	//	}
	//}


}
