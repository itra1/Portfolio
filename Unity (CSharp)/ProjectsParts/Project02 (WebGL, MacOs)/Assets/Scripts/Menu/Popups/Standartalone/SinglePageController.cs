using it.Main;
using System.Collections;
using UnityEngine;
using it.Main.SinglePages;
using it.Popups;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Net;
using static it.UI.SettingsLeftPanel;

namespace it.Main
{
	public class SinglePageController : Singleton<SinglePageController>
	{
		private List<SinglePage> _pages = new List<SinglePage>();

		public List<SinglePage> Pages { get => _pages; set => _pages = value; }

		private void Awake()
		{
			FindPages();
		}

		private void OnEnable()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.MainPageOpen, MainPageOpen);
		}

		private void OnDisable()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.MainPageOpen, MainPageOpen);
		}

		private void MainPageOpen(com.ootii.Messages.IMessage handle)
		{
			_pages.ForEach(x =>
			{
				if (x != null)
					Destroy(x.gameObject);
			});

			_pages.Clear();
		}

		private void FindPages()
		{
			_pages = gameObject.transform.GetComponentsInChildren<SinglePage>(true).ToList();
			_pages.ForEach(x =>
			{
				if (x != null)
					Destroy(x.gameObject);
			});
			_pages.Clear();
		}
		public T Show<T>(SinglePagesType type) where T : SinglePage
		{
			return Show(type) as T;
		}
		public SinglePage Show(SinglePagesType type)
		{
			it.Logger.Log("Open single page type " + type.ToString());

			_pages.RemoveAll(x => x == null);

			SinglePage popup = _pages.Find(x => x.PageType == type);
			if (popup != null)
				return popup;

			//var newPopupResource = Resources.Load<it.Main.SinglePages.SinglePage>($"Prefabs/UI/SinglePages/{type.ToString()}SP");
			var newPopupResource = ((GameObject)Garilla.ResourceManager.GetResource<GameObject>($"Prefabs/UI/SinglePages/{type.ToString()}SP")).GetComponent<it.Main.SinglePages.SinglePage>();
			if (newPopupResource == null) return null;

			GameObject inst = Instantiate(newPopupResource.gameObject, transform);

			popup = inst.GetComponent<SinglePage>();
			_pages.Add(popup);
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.SinglePageChnage);

			popup.Visible();
			popup.OnVisibleComplete = () =>
			{
				_pages.ForEach(x =>
				{
					if (x != null)
					{
						if (x.PageType != type)
							Destroy(x.gameObject);
					}
				});
			};
			popup.OnClose.AddListener(() =>
			{
				_pages.Remove(popup);
				Destroy(popup.gameObject);
				com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.SinglePageChnage);
			});

			return popup;
		}
	}
}