using System;
using System.Collections.Generic;
using BestHTTP;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Editor.Building
{
	public class TelegramView
	{
		private string _botKey => _settings.TelegramBotKey;

		private BuildWindow _window;
		private VisualElement _root;
		private List<Record> _records = new List<Record>();
		private VisualTreeAsset _itemPrefab;
		private ScrollView _scrollView;
		private BuildWindowSettings _settings;

		public TelegramView(BuildWindow window, BuildWindowSettings settings)
		{
			_window = window;
			_settings = settings;

			_itemPrefab ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/Building/TelegramChatRecord.uxml");

			_records.Clear();

			foreach (var chat in _settings.Telegrams)
			{
				_records.Add(new Record(chat, () => {
					_settings.Save();
				}));
			}

			VisibleRecords();
		}

		public void VisibleRecords()
		{
			_root ??= _window.Root;
			if (_root == null) return;

			_scrollView ??= _root.Q<ScrollView>("chatList");
			if (_scrollView == null) return;

			_scrollView.Clear();
			foreach (Record record in _records)
			{
				_scrollView.Add(record.GetRecord(_itemPrefab));
			}
			_scrollView.style.height = 0;
		}

		public void Message(string message)
		{
			foreach (Record record in _records)
			{
				//Debug.Log(record.IsSend);
				if (record.IsSend)
					Request(record.Id, message);
			}
		}

		private void Request(string chatId, string message)
		{
			var request = new HTTPRequest(new Uri($"https://api.telegram.org/{_botKey}/sendMessage"), HTTPMethods.Post, true, true, (req, resp) =>
			{
				switch (req.State)
				{
					// The request finished without any problem.
					case HTTPRequestStates.Finished:
						if (resp.IsSuccess)
						{
							Debug.Log("OnComplete");

						}
						else
						{
							Debug.Log(string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2} {3}",
																 resp.StatusCode,
																 resp.Message,
																 resp.DataAsText,
																 req.Uri.AbsoluteUri));
						}
						break;

					// The request finished with an unexpected error. The request's Exception property may contain more info about the error.
					case HTTPRequestStates.Error:
						Debug.LogError("Request Finished with Error! " + (req.Exception != null ? (req.Exception.Message + "\n" + req.Exception.StackTrace) : "No Exception"));
						break;

					// The request aborted, initiated by the user.
					case HTTPRequestStates.Aborted:
						Debug.Log("Request Aborted!");
						break;

					// Connecting to the server is timed out.
					case HTTPRequestStates.ConnectionTimedOut:
						Debug.LogError("Connection Timed Out!");
						break;

					// The request didn't finished in the given time.
					case HTTPRequestStates.TimedOut:
						Debug.LogError("Processing the request Timed Out!");
						break;
				}
			});

			request.SetHeader("Content-Type", "application/json");
			request.AddField("chat_id", chatId);
			request.AddField("parse_mode", "HTML");
			request.AddField("text", message);

			request.Send();
		}

	}

	class Record
	{
		public string Title => _chat.Title;
		public bool IsSend => _chat.IsSend;
		public string Id => _chat.Id;
		public VisualElement UIElement;
		
		private BuildWindowSettings.TelegramChat _chat;
		private UnityAction OnChnage;

		public Record(BuildWindowSettings.TelegramChat chat, UnityAction onChange)
		{
			_chat = chat;
			OnChnage = onChange;
		}

		public VisualElement GetRecord(VisualTreeAsset prefab)
		{
			UIElement ??= prefab.Instantiate();
			var recordLabel = UIElement.Q<Label>("label");
			var selectCheckbos = UIElement.Q<Toggle>("isSelect");
			recordLabel.text = Title;
			selectCheckbos.value = IsSend;

			selectCheckbos.RegisterValueChangedCallback<bool>((res) =>
			{
				selectCheckbos.value = res.newValue;
				_chat.IsSend = res.newValue;
				OnChnage?.Invoke();
			});

			return UIElement;

		}
	}
}