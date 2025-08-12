using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System;
using it.Network.Rest;
using it.Network.Socket;

public class SupportChatUI : MonoBehaviour
{


	[SerializeField] private TMP_InputField InputMessageField;
	[SerializeField] private Scrollbar scrollbar;
	[SerializeField] private Button buttonSend;
	[SerializeField] private VerticalLayoutGroup contentLayout;

	[Space]
	[SerializeField] private Transform MessagesParent;
	[SerializeField] private ChatMess MessagePref;
	[SerializeField] private ChatMess MessageRightPref;
	private List<ChatMess> Messes = new List<ChatMess>();

	private MessageRequest messages;
	private bool autoscroll;
	private ReferenceData referenceData;
	private bool init;
	private string _chanel;

	private void OnEnable()
	{
		Init();
	}



	public async void Init()
	{
		GetMessages();

		autoscroll = true;

		if (init == false)
		{
			init = true;
			//WebsocketClient.Instance.SupportMessageCreatedCallback += UpdateChat;
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.SupportChatMessageCreates(_chanel), UpdateChat);
		}

		MessEdit(InputMessageField.text);
	}

	private void UpdateChat(com.ootii.Messages.IMessage handle)
	{
		UpdateChat(((SupportChatMessageCreated)handle.Data).IsSupport);
	}
	private void UpdateChat(bool isUpdated)
	{
		GetMessages();
	}

	private async void GetMessages()
	{
		var supportManager = await SupportChatManager.GetInstanceAsync();
		supportManager.GetSupportMessages("", (messages) =>
		{
			ChatCreate(messages);
		});
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Return) && Input.GetKey(KeyCode.LeftControl) == false)
		{
			PostChatFromField();
		}
	}

	public void Close()
	{

		//WebsocketClient.Instance.SupportMessageCreatedCallback -= UpdateChat;
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.SupportChatMessageCreates(_chanel), UpdateChat);
		autoscroll = false;
	}

	private void ChatCreate(MessageRequest messages)
	{
		Messes.ForEach(x => { Destroy(x.gameObject); });
		Messes.Clear();
		for (int i = 0; i < messages.data.Count; i++)
		{
			AddMessage(messages.data[i], false);
		}
	}

	private void ScrollDown()
	{
		if (autoscroll)
			scrollbar.value = 0;
	}

	public void SetAutoScroll(float vl)
	{
		autoscroll = vl == 0;
	}

	private void AddMessage(Datum message, bool first = true)
	{
		StartCoroutine(SetUpVerticalLayout());
		ChatMess mess;

		TableChatMessage nMes = new TableChatMessage();
		nMes.id = message.id;
		nMes.user_id = message.user_id;
		nMes.message = message.message;
		nMes.updated_at = message.updated_at.ToString();
		nMes.created_at = message.created_at.ToString();

		if (message.user_id == GameHelper.UserInfo.id)
		{
			mess = Instantiate(MessageRightPref, MessagesParent);
			nMes.user = GameHelper.UserInfo;
		}
		else
		{
			mess = Instantiate(MessagePref, MessagesParent);
			nMes.user = new User();
			nMes.user.nickname = message.@operator.display_name;
		}

		if (first)
		{
			mess.transform.SetAsFirstSibling();
		}
		Messes.Add(mess);



		mess.Init(nMes);

	}

	public async void PostChat(string mess)
	{
		autoscroll = true;
		SupportMessage theMes = new SupportMessage();
		theMes.message = mess;
		var supportManager = await SupportChatManager.GetInstanceAsync();
		supportManager.PostSupportMessage(theMes, (result) =>
		{
			//postingRequest = result;
		});

	}

	public void PostChatFromField()
	{
		PostChat(InputMessageField.text);
		InputMessageField.text = "";
	}



	public void MessEdit(string txt)
	{
		buttonSend.interactable = txt.Length > 0;
	}

	IEnumerator SetUpVerticalLayout()
	{
		yield return new WaitForEndOfFrame();

		contentLayout.enabled = false;

		contentLayout.CalculateLayoutInputVertical();
		Canvas.ForceUpdateCanvases();
		contentLayout.enabled = true;


	}
}




