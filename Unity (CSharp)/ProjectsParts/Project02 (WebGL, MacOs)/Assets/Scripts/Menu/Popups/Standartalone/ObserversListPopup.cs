using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Network.Rest;
using it.Popups;
using it.Network.Socket;

public class ObserversListPopup : PopupBase
{
	[SerializeField] private ObserverItemUI _playerSectionPref;
	[SerializeField] private UnityEngine.UI.ScrollRect _scroll;

	private PoolList<ObserverItemUI> _items;
	private Table _table;
	private string _chanel;

	private void OnDestroy()
	{
		RemoveSubscribe();
	}

	private void RemoveSubscribe()
	{
		if (!string.IsNullOrEmpty(_chanel))
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.ObsorveListUpdate(_chanel), UpdateObsorveList);
	}

	public void SetData(Table table)
	{
		if (_items == null)
			_items = new PoolList<ObserverItemUI>(_playerSectionPref.gameObject, _scroll.content);
		_playerSectionPref.gameObject.SetActive(false);
		_table = table;

		RemoveSubscribe();

		_chanel = SocketClient.GetChanelTable(_table.id);

		//WebsocketClient.Instance.ObserversUpdateCallback += UpdateObserversCount;

		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ObsorveListUpdate(_chanel), UpdateObsorveList);

		ObserversUpdate();
	}

	private void UpdateObsorveList(com.ootii.Messages.IMessage handle)
	{
		UpdateObserversCount(((ObserversListUpdated)handle.Data).Obsorves);
	}

	public void ObserversUpdate(UsersLimitedRespone usersLimitedRespone = null)
	{
		if (gameObject.activeInHierarchy == false) return;
		if (usersLimitedRespone == null) GameHelper.GetObservers(_table, ShowObservers);
		else ShowObservers(usersLimitedRespone);
	}

	public void UpdateObserversCount(ObserversUsersRespone observersRespone)
	{
		ObserversUpdate(observersRespone.GetUsersLimitedRespone);
	}

	public void ShowObservers(UsersLimitedRespone userLimitedRespone)
	{
		UserLimited[] userLimiteds = userLimitedRespone.data;
		_items.HideAll();

		RectTransform rtPrefab = _playerSectionPref.GetComponent<RectTransform>();

		for (int i = 0; i < userLimiteds.Length; i++)
		{
			AddUser(userLimiteds[i]);
		}
		_scroll.content.sizeDelta = new Vector2(_scroll.content.sizeDelta.x, rtPrefab.rect.height * userLimiteds.Length);
	}

	public void AddUser(UserLimited user)
	{
		var item = _items.GetItem();
		item.Init(user);
	}
}
